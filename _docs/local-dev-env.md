# Local Dev

> Note:
>
> `run-local.sh` might fail first time running in new terminal, because it starts docker mssql instance and sometimes it will compile and run quicker than starting mssql docker instance, in that case, run same command again.

*   [First time docker](#first-time-docker)
*   [Quick Run](#quick-run)
*   [Step by Step Run](#step-by-step-run)
*   [Using API](#using-api)
  *   [Swagger](#swagger)
  *   [Postman](#postman)

## First time docker

```shell
cd <src-root>
./docker-mssql-run.sh
```

This a self-signed certificate and most browsers will complain. On `Windows` and `macOS` there is possibility to add this`*.pfx` key to KeyChain. On `linux` add exception to browser.

## Quick Run

Run _VS Code_ with _Debug (F5)_ or run _Task_ named `run`.

Or in terminal run:

```shell
cd <src-root>
./run-local.sh
```

### Step by Step Run

For local run without docker for app, but for `mssql` (make sure docker service is running) use this commands:

```shell
cd <src-root>
docker start sql1
# Set env variables for local dev (or add to bashrc, zshrc, etc, then this is not necessary)
export ASPNETCORE_ENVIRONMENT=Local
export ASPNETCORE_URLS='http://0.0.0.0:5000'
export DB_HOST=localhost
export DB_NAME=test-local-new
export DB_USER=sa
export DB_PASSWORD="Abc\$12345"
export JWT_AUTHORITY=https://dev-ajj38rm9.auth0.com
export JWT_AUDIENCE=https://ic.x430n.com
dotnet build
cd api
# On run will run migrations if DB not existing or not updated to latest migration
dotnet run
```

## Using API

Make sure having API running with either [run-local.sh](../run-local.sh) or _VS Code._

### Swagger

Navigate to home url, and choose version of API to try:

```text
http://localhost:5000

# or SSL version
https://localhost:5001
```

Authorize API for swagger with these steps:

*   Get up-to-date [JWT Token](jwt-token.md).
*   At right side of page click button `Authorize`.
*   Write down `Bearer <paste-generated-token>`. Make sure to have space between _Bearer_ and JWT token.

> Note:
>
> Need for JWT Token can be removed all together by removing `[Authorize]` in [BaseController](../src/api/BaseController.cs) attribute and re-running API.

### Postman

Run all API calls through Postman.

Import postman collection `InstantCoach API.postman_collection.json` from `_postman` folder.

> Note:
>
> `Id` used for requests is integer `1`, if database is empty, first run Create (_POST_) request. If database is not empty and there is no `Id` of integer `1`, change it to existing number, use List (_GET_) to get existing `Id`'s.

> Note:
>
> To add Authorization to all request in postman collection, follow these steps:
> *   Hover right of the _InstantCoach API_ collection in sidebar.
> *   click _..._ and choose _Edit_.
> *   With open _Edit Collection_ modal window, choose second tab _Authorization_.
> *   From type choose _Bearer Token_.
> *   In _Token_ textbox paste up-to-date [JWT Token](jwt-token.md).