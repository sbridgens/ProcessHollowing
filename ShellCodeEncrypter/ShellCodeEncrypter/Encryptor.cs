using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ShellCodeEncrypter
{
    class Encryptor
    {
        public byte[] AesEncrypt(byte[] input)
        {
            PasswordDeriveBytes pdb =
              new PasswordDeriveBytes("ic34xe!!!",//change this
              new byte[] { 0x67, 0x65, 0x74, 0x66, 0x75, 0x63, 0x6b, 0x65, 0x64 });//change this
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.KeySize = 256;
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        public byte[] AesDecrypt(byte[] input)
        {
            PasswordDeriveBytes pdb =
              new PasswordDeriveBytes("ic34xe!!!",//change this
               new byte[] { 0x67, 0x65, 0x74, 0x66, 0x75, 0x63, 0x6b, 0x65, 0x64 });//change this
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.KeySize = 256;
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms,
              aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        public void CaesarEncryptShell(byte[] shellcode)
        {
            Console.WriteLine($"[+] Caesar Encoding Payload");
            byte[] encoded = new byte[shellcode.Length];
            //caesar
            for (int i = 0; i < shellcode.Length; i++)
            {
                //5 iterations
                encoded[i] = (byte)(((uint)shellcode[i] + 5) & 0xFF);//5 iterations so ammend as needed
            }

            Console.WriteLine($"[+] Payload Is Caesar Encoded");

            Console.WriteLine("[+] Encrypting Payload using AES 256!");
            encoded = AesEncrypt(encoded);

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
