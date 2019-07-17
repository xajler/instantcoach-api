# InstantCoach app


[![Travis Build Status](https://travis-ci.org/xajler/instantcoach-api.svg)](https://travis-ci.org/xajler/instantcoach-api)
[![Coverage Status](https://coveralls.io/repos/github/xajler/instantcoach-api/badge.svg?branch=master)](https://coveralls.io/github/xajler/instantcoach-api?branch=master)

Using ASP.NET Core API for sample microservices REST API.


![Swagger](https://git.430n.com/x430n/instantcoach/raw/branch/master/_assets/swagger.png)

Unit testing with code coverage in VS Code

![unit_testing](https://git.430n.com/x430n/instantcoach/raw/branch/master/_assets/unit_testing_code_coverage.png)

Domain Errors and Endpoint Request Logging

![domain_errors](https://git.430n.com/x430n/instantcoach/raw/branch/master/_assets/domain_errors_logging.png)


> Note:
>
> Most of C# files contain multiple classes, because of microservices nature, if this wouldn't be microservice, those would go to folders and separate files.

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

## Features

* EF Migrations
* Config through IOptions&lt;T&gt;
* Docker MSSQL
* API Versioning
* REST API Endpoint (multiple versions, JWT Auth)
* Swagger (multiple versions, JWT Auth)
* Db CRUD / Service
* JWT Auth
* Error Handling (Known Errors Result and Result&lt;T&gt;, Global Exception through Middleware)
* Logging (including adding response time in ms in logs and response header as X-Response-Time through Middleware)
* Domain Models Validation
* Unit Testing (Domain)
* Refactoring (Domain to DDD and separate project)
* Integration Testing (Repositories and Controllers V1)
* Dockerfile Build/Publish/Run App
* Dockerfile Nginx web server with SSL nginx.conf
* Docker Compose (Development with watch, Test)
* SSL (development: dotnet dev-certs https, test: nginx self-signed certificate)

## TODO

* Unit Testing - Mock Services, problem Repository is not interface?
* Health checks
* GraphQL (? maybe separate project)
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
