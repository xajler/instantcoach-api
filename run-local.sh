#!/bin/sh


# Make sure that docker is running as service in linux
# sudo systemctl start docker.service
export ASPNETCORE_ENVIRONMENT=Local
export ASPNETCORE_URLS='http://0.0.0.0:5000'
export DB_HOST=localhost
export DB_NAME=test-local-new
export DB_USER=sa
export DB_PASSWORD="Abc\$12345"
export JWT_AUTHORITY=https://dev-ajj38rm9.auth0.com
export JWT_AUDIENCE=https://ic.x430n.com
docker start sql1
cd src/api
dotnet run
