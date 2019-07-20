# Local Dev

> Note:
>
> `docker-run.sh` might fail first time running in new terminal, because it starts docker mssql instance and sometimes it will compile and run quicker than starting mssql docker instance, in that case, run same command again.

## First time docker

    cd <src-root>
    docker pull microsoft/mssql-server-linux
    ./docker-mssql-run.sh

## SSL

To run locally with SSL on port (5001), make sure to run this command to create local dotnet core certificate:

    dotnet dev-certs https -ep ${HOME}/.aspnet/https/instant-coach-api.pfx -p bm8kpv@=n2y4Nz@#

This a self-signed certificate and most browsers will complain. On `Windows` and `macOS` there is possibility to add this`*.pfx` key to keychain. On `linux` add exception to browser.

## Quick Run

Run _VS Code_ with _Debug (F5)_ or run _Task_ named `run`.

Or in terminal run:

    cd <src-root>
    ./run-local.sh

### Step by Step Run

For local run without docker for app, but for `mssql` (make sure docker service is running) use this commands:

    cd <src-root>
    docker start sql1
    # Set env variables for local dev (or add to bashrc, zshrc, etc, then this is not necessary)
    source ./env-var.sh
    dotnet build
    cd api
    # On run will run migrations if DB not existing or not updated to latest migration
    dotnet run

## Using API

Make sure having API running with either [run-local.sh](../run-local.sh) or _VS Code._

### Swagger

Navigate to home url, and choose version of API to try:

    http://localhost:5000

    # or SSL version
    https://localhost:5001

Authorize API for swagger with these steps:

* Get up-to-date [JWT Token](jwt-token.md).
* At right side of page click button `Authorize`.
* Write down `Bearer <paste-generated-token>`. Make sure to have space between _Bearer_ and JWT token.

> Note:
>
> Need for JWT Token can be removed all together by removing `[Authorize]` in [BaseController](../api/BaseController.cs) attribute and re-running API.


### Postman

Run all API calls through Postman.

Import postman collection `InstantCoach API.postman_collection.json` from `_postman` folder.

>Note:
>
> `Id` used for requests is integer `1`, if database is empty, first run Create (_POST_) request. If database is not empty and there is no `Id` of integer `1`, change it to existing number, use List (_GET_) to get existing `Id`'s.

>Note:
>
> To add Authorization to all request in postman collection, follow these steps:
> * Hover right of the _InstantCoach API_ collection in sidebar.
> * click _..._ and choose _Edit_.
> * With open _Edit Collection_ modal window, choose second tab _Authorization_.
> * From type choose _Bearer Token_.
> * In _Token_ textbox paste up-to-date [JWT Token](jwt-token.md).