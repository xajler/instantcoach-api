#!/bin/sh

export ASPNETCORE_ENVIRONMENT=Local
export ASPNETCORE_URLS='http://0.0.0.0:5000;https://0.0.0.0:5001'
export DB_HOST=localhost
export DB_NAME=test-local-new
export DB_USER=sa
export DB_PASSWORD="Abc\$12345"
export JWT_AUTHORITY=https://dev-ajj38rm9.auth0.com
export JWT_AUDIENCE=https://ic.x430n.com
export ASPNETCORE_Kestrel__Certificates__Default__Password=bm8kpv@=n2y4Nz@#
export ASPNETCORE_Kestrel__Certificates__Default__Path=/home/x/.aspnet/https/instant-coach-api.pfx
