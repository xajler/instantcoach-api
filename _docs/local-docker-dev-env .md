# Local Docker Dev

Using local folder `<src-root>` mapped to `/app` docker container volume.

> Note:
>
> MS Build uses [Directory.Build.props](../Directory.Build.props) and uses `container` folder, to differentiate _local_ from _local docker_ build and publish (e.g. for docker build `bin/container/Debug/netcoreapp2.2` and for local build: `bin/Debug/netcoreapp2.2`).


## First time docker

    cd <src-root>
    docker-compose -f docker-compose-dev.yml build

## SSL

> Note:
>
> If you created SSL certificate by running [Local Dev](local-dev-env.md), this step can be skipped.


To run docker container with SSL on port (5001), make sure to run this command to create local dotnet core certificate:

    dotnet dev-certs https -ep ${HOME}/.aspnet/https/instant-coach-api.pfx -p bm8kpv@=n2y4Nz@#

This a self-signed certificate and most browsers will complain. On `Windows` and `macOS` there is possibility to add this`*.pfx` key to keychain. On `linux` add exception to browser.

> Note:
>
> `${HOME}/.aspnet/https` is mapped to `/https` docker container volume.

## Start Docker Dev Environment

It would be better to start docker compose in detached mode `-d`, but it can be omitted, if there is need to look at all containers logs simultaneously.

    cd <src-root>
    docker-compose -f docker-compose-dev.yml up -d
    # Check that all are running with
    docker ps -a
    # If any exited and stopped use logs to check for problems
    docker logs <container> -f

At this point API will be running with `watch`, meaning, with each save of code, it will re-run API app. Since each API request will produce console logs, run logs for API container while changing and testing code with this command:

    docker logs <api-container> -f


## Using API

Make sure [docker compose for dev](../docker-compose-dev.yml) is up and running.

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