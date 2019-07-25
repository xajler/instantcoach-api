#!/bin/sh


# Make sure that docker is running as service in linux
# sudo systemctl start docker.service

docker-compose -f docker/dev-unit-testing/docker-compose.yml up --build