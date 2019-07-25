#!/bin/sh

docker ps -a | grep ci_ | awk '{print $1}' | xargs docker stop
docker ps -a | grep ci_ | awk '{print $1}' | xargs docker rm
docker images | grep .ci | awk '{print $3}' | xargs docker image rm