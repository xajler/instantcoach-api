#!/bin/sh

if [ "$1" =  "master" ]
then
    docker-compose -f docker-compose-testing.yml up --build --exit-code-from api.sut
else
    docker build -f tests/tests-unit/Dockerfile .
fi