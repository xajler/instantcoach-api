#!/bin/sh


# Make sure that docker is running as service in linux
# sudo systemctl start docker.service

# Set Stage as ASPNETCORE_ENVIRONMENT possible values: 'Development', 'Staging', and 'Production'.
# Run as:
# ./run-env-docker.sh <one-of-those-three-stages>
docker-compose -f docker/docker-compose.yml build --build-arg ASPNETCORE_ENVIRONMENT=$1
docker-compose -f docker/docker-compose.yml up -d