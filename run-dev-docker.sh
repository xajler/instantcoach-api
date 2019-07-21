#!/bin/sh


# Make sure that docker is running as service in linux
# sudo systemctl start docker.service

docker-compose -f docker-compose-dev.yml build
docker-compose -f docker-compose-dev.yml up -d