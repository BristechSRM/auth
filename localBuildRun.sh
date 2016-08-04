#!/bin/bash -euv
echo -e '\033]2;'Auth'\007'
./build.sh
./copySecrets.sh
./run.sh
