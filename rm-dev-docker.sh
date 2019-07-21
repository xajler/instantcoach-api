#!/bin/sh

 docker ps -a | grep dev_ | awk '{print $1}' | xargs docker stop
 docker ps -a | grep dev_ | awk '{print $1}' | xargs docker rm
 docker images | grep .dev | awk '{print $3}' | xargs docker image rm

 # good one removes first 3 rows and last 2
 # dotnet list package | awk 'FNR > 3 { print $2}' | head -n -2
