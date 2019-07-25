FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS ci-build
#FROM x430n/dotnet-java-sonarscanner:1.0 as ci-build

#COPY .git ./.git
COPY src ./src
COPY tests  ./tests
COPY tools ./tools
COPY instantcoach.sln ./

ENTRYPOINT ["/bin/sh", "./tests/run-all-tests.sh"]