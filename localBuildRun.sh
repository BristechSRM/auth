#!/bin/bash -euv
echo -e '\033]2;'Auth'\007'
./setup.sh
./build.sh
./copySecrets.sh
./run.sh
