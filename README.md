# InstantCoach app


[![Travis Build Status](https://travis-ci.org/xajler/instantcoach-api.svg)](https://travis-ci.org/xajler/instantcoach-api)
[![Coverage Status](https://coveralls.io/repos/github/xajler/instantcoach-api/badge.svg?branch=master)](https://coveralls.io/github/xajler/instantcoach-api?branch=master)

Using ASP.NET Core API for sample microservices REST API.


![Swagger](_assets/swagger.png)

Unit testing with code coverage in VS Code

![unit_testing](_assets/unit_testing_code_coverage.png)

Domain Errors and Endpoint Request Logging

![domain_errors](_assets/domain_errors_logging.png)


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
* Github badges for Code Coverage (coveralls.id) and CI (Travis CI) (master branch)
* Integrated Elasticsearch APM (Application Performance Monitoring)

## TODO

* Unit Testing - Mock Services, problem Repository is not interface?
* Redis cache Docker or Nginx
* Health checks
* Apiary
* Storyteller tests (?)
* GraphQL (? maybe separate project)
* CD Azure (? only one I have access to deploy)
* Message Queue (? maybe separate project)
* CQRS (? maybe separate project)

## Run

### Local Dev

Runs with locally installed _.NET Core SDK_. Uses only _MSSQL_ as a _Docker_ container.

Find out more how to run [Local Dev](_docs/local-dev-env.md).

### Docker Dev

Runs all services as _docker_ containers, but with mounted _docker volumes_ to code repository and _https certificate_. Has `ASPNETCORE_ENVIRONMENT` set to `Development`, and uses _APM Server_ with _ElasticSearch LogStash_ as main development driver

Find out more how to run [Docker Dev](_docs/docker-dev-env.md).


### Docker Testing/Production

Similar to [Docker Dev](_docs/docker-dev-env.md) but not sharing local machine folder(s) as docker container volumes. Everything runs inside of docker containers.

TODO

## APM Server

[Docker Dev](docs/docker-dev-env.md) and Testing can use _Elasticsearch APM (Application Performance Monitor) Server_ for monitoring errors and requests and system metrics. At this point in time _.NET Core Agent for APM Server_ is not production ready, but it is of great value for developers. It can be great to for stress testing with _jMeter_ or _Postman_ runner.