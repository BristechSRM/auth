cd build/output/
if test "$OS" = "Windows_NT"
then
    ./Auth.exe
else
    mono Auth.exe
fi
