#!/bin/sh

 docker ps -a | grep sut_ | awk '{print $1}' | xargs docker stop
 docker ps -a | grep sut_ | awk '{print $1}' | xargs docker rm
 docker images | grep .sut | awk '{print $3}' | xargs docker image rm
