#!/bin/sh

sudo docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Abc$12345' \
   -p 1433:1433 --name sql1 \
   -d mcr.microsoft.com/mssql/server:2017-GDR-ubuntu
