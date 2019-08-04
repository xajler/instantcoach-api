FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS ci-build-unit

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

WORKDIR /app

COPY src ./src
COPY tests  ./tests/
COPY instantcoach.sln .

WORKDIR /app/tests/tests-unit

ENTRYPOINT ["./run-unit-tests.sh"]