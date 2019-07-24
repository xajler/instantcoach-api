FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS test

RUN apt-get update
RUN apt-get install default-jdk -y --no-install-recommends

ENV PATH="$PATH:/root/.dotnet/tools"
COPY .git ./.git
COPY api ./api/
COPY core ./core/
COPY domain ./domain/
COPY tests  ./tests/
COPY tools ./tools/
COPY instantcoach.sln ./
RUN dotnet tool install --global dotnet-sonarscanner

RUN dotnet restore
RUN dotnet build

ENTRYPOINT ["/bin/sh", "./tests/run-all-tests.sh"]