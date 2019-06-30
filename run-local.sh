#!/bin/sh


# Make sure that docker is running as service in linux
# sudo systemctl start docker.service
. ./EnvVar.sh
docker start sql1
cd api
dotnet run
