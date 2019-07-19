FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS test

COPY api/. ./api/
COPY core/. ./core/
COPY domain/. ./domain/
COPY tests  ./tests/
COPY tools ./tools/
COPY instantcoach.sln ./
RUN dotnet restore
RUN dotnet build

ENTRYPOINT ["/bin/sh", "./tests/run-all-tests.sh"]