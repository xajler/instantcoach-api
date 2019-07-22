## .NET Core

Best to install specific version via `dotnet-install` scripts, for unix/linux:

```shell
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install
# or 2.1, also -Version can be applied if particular version needed
./dotnet-install.sh -Channel 2.2
```

.NET Core will be installed in `~/.dotnet` add this to your `$PATH` in .bashrc, .zshrc, etc

```shell
export PATH=$HOME/.dotnet:$PATH
```

## Docker

### MSSQL (linux)

Make sure on linux to create `docker` group and add dev user to this group.

```shell
docker pull microsoft/mssql-server-linux
# Use docker-run.sh or copy this command
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Abc$12345' \
            -p 1433:1433 --name sql1 \
            -d microsoft/mssql-server-linux
# make sure running
docker ps -a

# stop mssql instance
docker stop sql1
# start mssql instance
docker start sql1
```

### Install mssql-tools debian docker image

```shell
apt-get update
apt install vim locales gnupg2 apt-transport-https wget iputils-ping
v /etc/locale.gen
curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | tee /etc/apt/sources.list.d/msprod.list
apt-get update
apt-get install mssql-tools unixodbc-dev

ldd /opt/microsoft/msodbcsql17/lib64/libmsodbcsql-17.3.so.1.1
wget http://security-cdn.debian.org/debian-security/pool/updates/main/o/openssl/libssl1.0.0_1.0.1t-1+deb8u11_amd64.deb
dpkg -i libssl1.0.0_1.0.1t-1+deb8u11_amd64.deb
ldd /opt/microsoft/msodbcsql17/lib64/libmsodbcsql-17.3.so.1.1
echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc

# Test few different examples
sqlcmd -S localhost -U SA -P 'Abc$12345'
sqlcmd -S 127.0.0.1 -U sa -P 'Abc$12345'
sqlcmd -S 127.0.0.1,1433 -U sa -P 'Abc$12345'
sqlcmd -S 10.5.0.2 -U sa -P 'Abc$12345'
sqlcmd -S 10.5.0.2,1433 -U sa -P 'Abc$12345'
```

## Entity Framework

Make sure that WebApi or class library project have this packages installed:

```
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### .NET SDK 2.0

Note: package `Microsoft.EntityFrameworkCore.Tools.DotNet` is part of .NET SDK 2.1 and above, so it only refers to .NET SDK 2.0.

```
dotnet add package Microsoft.EntityFrameworkCore.Tools.DotNet
```

If class lib is created make sure to change `TargetFramework` from `netstandard2.0` to `netcoreapp2.0`, because EF CLI cannot work with .NET Standard.

Make sure that is `DotNetCliToolReference` and remove it as `PackageReference` if it is set, this is how it should only be:

```xml
<ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.3" />
</ItemGroup>
```

### Migrations

Before running EF CLI locally make sure on each opening terminal instance to run (or add to bashrc, zsrhc, etc, then this is not necessary):

```shell
cd <src-root>
source ./env-var.sh
```

Change dir to directory having EF DbContext, either api project or class lib, in this case it is class lib folder and project `core` and going via webapi project that is startup project `api`:

```shell
cd core
# -s means startup project 'api' dir in this case
dotnet ef migrations add Initial -s ../api

# If there any errors remove migrations, fix errors then run add again
dotnet ef migrations remove

# Make sure no errors on creating initial migration
# Make sure you're running docker mssql instance!!!
# Then run this command to create database
dotnet ef database update -s ../api

# To drop database and start from scratch
dotnet ef database drop -s ../api

# To get DB script
dotnet ef migrations script -s ../api
```

Connection string using `sa` user and password for docker, good for dev and testing, but for production there is `db-create.sh` good MSSQL template for creating in production environment.

### SQL Script to create db user for app

Usually when using `sa` DB user, with EF it will create database and tables, this is not best solution for production, `sa` should only create database, and new user with less privileges should be used for app DB User, this is a script that can be used for such purpose. Especially this kind of script can be created and called in Dockerfile when creating new mssql container.

```sql
------------
-- 1st part
------------

USE master;

-- SELECT * FROM sys.database_principals;

-- In master db
CREATE LOGIN [<DbUser>] WITH PASSWORD = '<password>'
CREATE USER [<DbUser>] FOR LOGIN [<DbUser>]

CREATE DATABASE <db_name>;

------------
-- 2nd part
------------

USE <db_name>;

-- -- In user db
CREATE USER [<DbUser>] FOR LOGIN [<DbUser>]
ALTER ROLE [db_owner] ADD MEMBER [<DbUser>]

-- Check for ownership on user db
-- select m.name as Member, r.name as Role
-- from sys.database_role_members
-- inner join sys.database_principals m on sys.database_role_members.member_principal_id = m.principal_id
-- inner join sys.database_principals r on sys.database_role_members.role_principal_id = r.principal_id
```