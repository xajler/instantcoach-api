#!/bin/sh

if [ -d "/opt/mssql-tools/bin" ]; then
    /opt/mssql/bin/sqlservr &
    sleep 60s
    cat /etc/hosts
    ./opt/mssql-tools/bin/sqlcmd -S 127.0.0.1 -U sa -P 'Abc$12345' -i create-db.sql
    ./opt/mssql-tools/bin/sqlcmd -S 127.0.0.1 -U sa -P 'Abc$12345' -i create.sql
    apt-get remove -y --auto-remove --purge msodbcsql mssql-tools curl apt-transport-https debconf-utils
    # Very Hackety Hack...
    while true; do sleep 1000; done
else
    /opt/mssql/bin/sqlservr
fi