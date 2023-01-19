using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ShellCodeEncrypter
{
    public class Encryptor
    {
        private byte[] DeriveAesKeyAndIv(string password, byte[] iv, int keySize)
        {
            var pdb = new PasswordDeriveBytes(password, iv);
            Aes aes = new AesManaged();
            aes.KeySize = keySize;
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            return aes;
        }

        public byte[] AesEncrypt(byte[] input, string password, byte[] iv, int keySize)
        {
            var aes = DeriveAesKeyAndIv(password, iv, keySize);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        public byte[] AesDecrypt(byte[] input, string password, byte[] iv, int keySize)
        {
            var aes = DeriveAesKeyAndIv(password, iv, keySize);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }
        
        public byte[] CaesarEncrypt(byte[] input, int iterations)
        {
            byte[] encoded = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                encoded[i] = (byte)(((uint)input[i] + iterations) & 0xFF);
            }
            return encoded;
        }

        public void CaesarEncryptShell(byte[] shellcode, string password, byte[] iv, int keySize)
        {
            Console.WriteLine($"[+] Caesar Encoding Payload");
            byte[] encoded = CaesarEncrypt(shellcode, 5);
            Console.WriteLine($"[+] Payload Is Caesar Encoded");

            Console.WriteLine("[+] Encrypting Payload using AES 256!");
            encoded = AesEncrypt(encoded, password, iv, keySize);

            Console.WriteLine("[+] Hex Encoding AES Payload");
            StringBuilder hex = new StringBuilder(encoded.Length * 2);
            foreach (byte b in encoded)
            {
                hex.AppendFormat("0x{0:x2},", b);
            }
            Console.WriteLine($"[+] The Hex Encoded payload is: \r\n\r\n{hex.ToString()}");
            //Uncomment if you want a b64 version
            //Console.WriteLine("[+] Base64 Encoded Version:\r\n");
            //Console.WriteLine(Convert.ToBase64String(encoded));
        }

        //uncomment to test decoding.
        //private void Decode(string b64, string password, int rotationValue)
        //{
        //    var byteArray = Convert.FromBase64String(b64);
        //    var decrypted = AesDecrypt(byteArray);
        //    byte[] decoded = new byte[decrypted.Length];

        //    for (int i = 0; i < decrypted.Length; i++)
        //    {
        //        decoded[i] = (byte)(((uint)decrypted[i] - rotationValue) & 0xFF);
        //    }

        //    StringBuilder hex = new StringBuilder(decoded.Length * 2);
        //    foreach (byte b in decoded)
        //    {
        //        hex.AppendFormat("0x{0:x2}, ", b);
        //    }

        //    Console.WriteLine($"[+] The Hex Decoded payload is: \r\n{hex.ToString()}");
        //}
    }
}
