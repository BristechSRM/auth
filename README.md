auth
===

Certificate file
---

You can generate a self-signed certificate for use by Auth.

On Windows Command Prompt use makecert:
N.B.
 "^" is the DOS line continuation character, just for clarity
 CN= is "Common Name".  Just pick one.
 -r is for self signed.

> makecert.exe ^
-n "CN=BertAndEthel" ^
-r ^
-pe ^
-a sha512 ^
-len 4096 ^
-cy authority ^
-sv BertAndEthel.pvk ^
BertAndEthel.cer

it will prompt twice to set a password for the file.


You need it in pfx format, which you can create with pvk2pfx.
> pvk2pfx.exe ^
-pvk BertAndEthel.pvk ^
-spc BertAndEthel.cer ^
-pfx BertAndEthel.pfx

it will prompt for the file password.


More details here:
https://www.jayway.com/2014/09/03/creating-self-signed-certificates-with-makecert-exe-for-development/
