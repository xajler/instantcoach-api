# InstantCoach app

Using ASP.NET Core API for sample microservices REST API.


![Swagger](https://git.430n.com/x430n/instantcoach/raw/branch/master/_assets/swagger.png)

Unit testing with code coverage in VS Code

![unit_testing](https://git.430n.com/x430n/instantcoach/raw/branch/master/_assets/unit_testing_code_coverage.png)

Domain Errors and Endpoint Request Logging

![domain_errors](https://git.430n.com/x430n/instantcoach/raw/branch/master/_assets/domain_errors_logging.png)


> Note:
>
> Most of C# files contain multiple classes, because of microservices nature, if this wouldn't be microservice, those would go to folders and separate files.
>
> README state at this point is very chaotic, it will change when Dockerfile's are created for all. Then most of this README will go to separate file NOTES, since it will represent local notes, and most of it will be part of Dockerfile.

Created using:

* Linux (ArchLinux)
* .NET Core 2.2 (Local and Docker)
* C#7
* EF Core (Commands) / ADO.NET via EF Core (Queries)
* SQL Server 2017 (Docker)
* Nginx (Docker)
* VS Code (With C# Extensions)
* Azure Data Studio (Local GUI for SQL Server)
* Unit Testing (xUnit, FluentAssertions, Coverlet, Moq)

Unit Testing will come last, because there is possibility of major changes and refactoring throughout creating this microservices API architecture.

## TODO Features

* ~~EF Migrations~~
* ~~Config through IOptions&lt;T&gt;~~
* ~~Docker MSSQL~~
* ~~API Versioning~~
* ~~REST API Endpoint (multiple versions, JWT Auth)~~
* ~~Swagger (multiple versions, JWT Auth)~~
* ~~Db CRUD / Service~~
* ~~JWT Auth~~
* ~~Error Handling (Known Errors Result and Result&lt;T&gt;, Global Exception through Middleware)~~
* ~~Logging (including adding response time in ms in logs and response header as X-Response-Time through Middleware)~~
* ~~Domain Models Validation~~ (Maybe refactor to FluentValidations?)
* ~~Unit Testing (Only Domain, other projects will be Integration Tests)~~
* ~~Refactoring (Domain to DDD and separate project)~~ TODO: Mock Services
* Integration Testing (~~Repositories~~ TODO: WebAPI with InMemory EF)
* Dockerfile MSSQL
* Dockerfile nginx
* Dockerfile Build/Publish/Run App
* Docker Compose
* Health checks
* Nginx web server
* GraphQL (? maybe separate project)
* SSL (?)
* CI/CD Bitbucket pipelines (?)
* Message Queue (? maybe separate project)
* CQRS (? maybe separate project)

## Run

### Local Env

> Note:
>
> `docker-run.sh` might fail first time running in new terminal, because it starts docker mssql instance and sometimes it will compile and run quicker than starting mssql docker instance, in that case, run same command again.

#### First time docker

    cd <src-root>
    docker pull microsoft/mssql-server-linux
    ./docker-run.sh

### Quick usage

Run _VS Code_ with _Debug (F5)_ or run _Task_ named `run`.

Or in terminal run:

    cd <src-root>
    ./run-local.sh

#### Usual usage

For local run without docker for app, but for `mssql` (make sure docker service is running) use this commands:

    cd <src-root>
    docker start sql1
    # Set env vars for local dev (or add to bashrc, zsrhc, etc, then this is not necessary)
    source ./env-var.sh
    cd api
    dotnet build
    # On run will run migrations if DB not existing or not updated to latest migration
    dotnet run

### Swagger

Make sure having app running (currently only locally see more in `Usual usage` section).

Navigate to home url, and choose version of API to try:

    http://localhost:5000

To properly run, click in each action at right end lock, and add this token:

    eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ik9EaEVPRFE1TmtJM09VSkJNRGd3TkVJMk1qTTRPREE1TVRWRk0wTXpNekEzUkRVM1JrVkNSZyJ9.eyJpc3MiOiJodHRwczovL2Rldi1hamozOHJtOS5hdXRoMC5jb20vIiwic3ViIjoiNEJXaU4zNlNXR1RMOWR4a295MkRwQkpDVG1LT2ZPcUFAY2xpZW50cyIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImlhdCI6MTU2MjUwNDg2MSwiZXhwIjoxNTYzMTA5NjYxLCJhenAiOiI0QldpTjM2U1dHVEw5ZHhrb3kyRHBCSkNUbUtPZk9xQSIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.AAXbTINarIDCqQU0lrkfCm5LogyZ95rQrW-2S8vdgXf52qXSXZnSfs5ECXPQksED_Pf5bM1zdstf2on6x-55fB8kbOIVEOSJ6hpOrEn5crwjNVb4PlE8k1Il08UX5SIqWWJHG_Q-aIqEyeJrxb8QmujcoRP5PeLnt-l4Pd_CSY5xN9eCxIwf_7w_xr4IGgdBE-Hi74X-9loACBArVVZ-GN6krfgOLPC0VePnoZQ0YxTr3NHR28IF2CPzOfVUByqX0L8W_fcdE0x_FqB4FTOanwBXK3Lu98Y37DXifpb8TzEUZm9lmkN1DF8Ryz6xmD1s0PP76NgVe_ZCNU_LGjCOww

> Note:
>
> I will try to change this token when is expired, currently set to be valid for a week.
> If not either comment in Api.BaseController Autorize attribute, or create you own [auth0 account](https://auth0.com) create API and change Env var `JWT_AUTHORITY` in env-var.sh.


### Postman

Run all API calls through Postman.

Import postman collection `InstantCoach API.postman_collection.json` from `_postman` folder.

>Note:
>
>JWT Authentication is applied, but if there is problem, pleas read note in previous Swagger section.

## JWT Authentication

Reason why is not used Username/Password in this implementation of JWT Authentication is because Frontend should not have direct access to REST API. It should have access to MVC Web App that would have User Identity through JWT token. Through that web app as Proxy Frontend would call REST API, sending User Identity JWT Token.

If in future MVC Web App would be created, proper JWT Authentication would be implemented and already created JWT token would be pass to REST API and created User Identity as Middleware.


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
    source ./env-var.sh

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
