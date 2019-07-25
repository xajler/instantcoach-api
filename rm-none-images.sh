#!/bin/sh

docker images | grep '<none>' | awk '{print $3}' | xargs docker image rm