auth
===

Auth needs some secret information that is not checked in.  This consists of secrets.config, and a certificate file containing a private key.

Secrets file
---
The secrets.config file should contain:
```
<appSettings>
    <add key="googleClientId" value="" />
    <add key="googleClientSecret" value="" />
    <add key="certificateFile" value="" />
    <add key="certificatePassword" value="" />
</appSettings>
```


Certificate file
---
You can generate a self-signed certificate for use by Auth.

On Windows Command Prompt use makecert:
N.B.
 "^" is the DOS line continuation character, just for clarity
 CN= is "Common Name".  Just pick one.
 -r is for self signed.
```
> makecert.exe ^
-n "CN=BertAndEthel" ^
-r ^
-pe ^
-a sha512 ^
-len 4096 ^
-cy authority ^
-sv BertAndEthel.pvk ^
BertAndEthel.cer
```
it will prompt twice to set a password for the file.


You need it in pfx format, which you can create with pvk2pfx.
```
> pvk2pfx.exe ^
-pvk BertAndEthel.pvk ^
-spc BertAndEthel.cer ^
-pfx BertAndEthel.pfx
-po  pfxfilepassword
```

It will prompt for the password of the pvk file.


More details here:
https://www.jayway.com/2014/09/03/creating-self-signed-certificates-with-makecert-exe-for-development/
