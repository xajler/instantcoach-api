#!/bin/sh

docker stop dev-unit-testing_api.dev.unit_1
docker rm dev-unit-testing_api.dev.unit_1
docker image rm dev-unit-testing_api.dev.unit
