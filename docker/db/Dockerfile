FROM mcr.microsoft.com/mssql/server:2017-GDR-ubuntu
ARG MSSQL_SA_PASSWORD
ENV MSSQL_SA_PASSWORD=$MSSQL_SA_PASSWORD
ENV ACCEPT_EULA=Y

# apt-get and system utilities
RUN apt-get update && apt-get install -y \
	curl apt-transport-https debconf-utils \
    && rm -rf /var/lib/apt/lists/*

# adding custom MS repository
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
RUN curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list > /etc/apt/sources.list.d/mssql-release.list

# install SQL Server drivers and tools
RUN apt-get update && ACCEPT_EULA=Y apt-get install -y msodbcsql mssql-tools

RUN apt-get -y install locales
RUN locale-gen en_US.UTF-8
RUN update-locale LANG=en_US.UTF-8

COPY create-db.sql ./
COPY create.sql ./

RUN ( /opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" \
    && /opt/mssql-tools/bin/sqlcmd -S 127.0.0.1 -U SA -P $MSSQL_SA_PASSWORD -i create-db.sql \
    && /opt/mssql-tools/bin/sqlcmd -S 127.0.0.1 -U SA -P $MSSQL_SA_PASSWORD -i create.sql \
    && pkill sqlservr

ENTRYPOINT ["/opt/mssql/bin/sqlservr"]