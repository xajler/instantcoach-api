# InstantCoach app

Using ASP.NET Core API for sample microservices REST API.

> Note:
>
> Most of C# files contain multiple classes, because of microservices nature, if this wouldn't be microservice, those would go to folders and separate files.
>
> README state at this point is very chaotic, it will change when Dockerfile's are created for all. Then most of this README will go to separate file NOTES, since it will represent local notes, and most of it will be part of Dockerfile.

Created using:

* Linux (ArchLinux)
* .NET Core 2.2 (Local and Docker)
* SQL Server 2017 (Docker)
* VS Code (With C# Extensions)
* Azure Data Studio (Local GUI for SQL Server)

Unit Testing will come last, because there is possibility of major changes and refactoring throughout creating this microservices API architecture.

## Features

* ~~EF Migrations~~
* ~~Config through IOptions&lt;T&gt;~~
* ~~Docker MSSQL~~
* ~~API Versioning~~
* ~~REST API Endpoint~~ (TODO: connect with CRUD and Auth)
* ~~Swagger~~ (TODO: Auth)
* Db CRUD / Service
* JWT Auth
* Error Handling
* Logging
* Unit Testing / Refactoring
* Health checks
* Dockerfile MSSQL
* Dockerfile Build/Run App
* Docker Compose
* Nginx web server
* GraphQL (?)
* SSL (?)
* CI/CD Bitbucket pipelines (?)
* Integration Testing (?)
* Message Queue (?)
* CQRS (?)

## Run

### Local Env

#### First time docker

    cd <src-root>
    docker pull microsoft/mssql-server-linux
    ./docker-run.sh

### Quick usage

Run VS Code with Debug or Task named `run`.

Or in terminal run:

    cd <src-root>
    ./run-local.sh

#### Usual usage

For local run without docker for app, but for `mssql` (make sure docker service is running) use this commands:

    cd <src-root>
    docker start sql1
    # Set env vars for local dev (or add to bashrc, zsrhc, etc, then this is not necessary)
    source ./EnvVar.sh
    cd api
    dotnet build
    # On run will run migrations if DB not existing or not updated to latest migration
    dotnet run

### Swagger

Make sure having app running (currently only locally see more in `Usual usage` section).
Navigate to url:

    http://localhost:5000/swagger


### Postman

Run all API calls through Postman.

Import postman collection `InstantCoach API.postman_collection.json` from `postman` folder.


## .NET Core

Best to install specific version via `dotnet-install` scripts, for unix/linux:

    wget https://dot.net/v1/dotnet-install.sh
    chmod +x dotnet-install
    # or 2.1, also -Version can be applied if particular version needed
    ./dotnet-install.sh -Channel 2.2

.NET Core will be installed in `~/.dotnet` add this to your `$PATH` in .bashrc, .zshrc, etc

    export PATH=$HOME/.dotnet:$PATH


## Docker

### MSSQL (linux)

Make sure on linux to create `docker` group and add dev user to this group.


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


## Entity Framework


Make sure that WebApi or class library project have this packages installed:

    dotnet add package Microsoft.EntityFrameworkCore.Design


### .NET SDK 2.0

Note: package `Microsoft.EntityFrameworkCore.Tools.DotNet` is part of .NET SDK 2.1 and above, so it only refers to .NET SDK 2.0.

    dotnet add package Microsoft.EntityFrameworkCore.Tools.DotNet

If class lib is created make sure to change `TargetFramework` from `netstandard2.0` to `netcoreapp2.0`, because EF CLI cannot work with .NET Standard.

Make sure that is `DotNetCliToolReference` and remove it as `PackageReference` if it is set, this is how it should only be:

    <ItemGroup>
        <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.3" />
    </ItemGroup>


### Migrations

Before running EF CLI locally make sure on each opening terminal instance to run (or add to bashrc, zsrhc, etc, then this is not necessary):

    cd <src-root>
    source ./EnvVar.sh

Change dir to directory having EF DbContext, either api project or class lib, in this case it is class lib folder and project `core` and going via webapi project that is startup project `api`:

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

Connection string using `sa` user and password for docker, good for dev and testing, but for production there is `db-create.sh` good MSSQL template for creating in production environment.
