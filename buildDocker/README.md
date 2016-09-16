#Building the Auth service 
To build, copy the contents of this folder to an empty directory on a linux VM. The VM must have docker installed.  
If you are already on linux, no need to do anything. 

Then simply run `./dockerBuild.sh` to build the docker image for Auth from the latest master.

If you want to test the build before something is in master, 
you'll need to change dockerBuild to clone a branch instead of master. The command for this is
`git clone --depth 1 -b my-branch https://github.com/BristechSRM/Auth.git $DIR/source`

## Pushing to docker
Make sure the image has been created correctly with a recent created time
`docker images auth:latest`

Now simply perform `docker login` with the credentials for bristechsrm and then run 
`docker push bristechsrm/auth`

Further details can be found here: 
https://docs.docker.com/engine/getstarted/step_six/ 