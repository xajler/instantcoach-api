FROM x430n/dotnet-sonarscanner:2.2 as ci-build

COPY .git ./.git
COPY src ./src
COPY tests  ./tests
COPY instantcoach.sln ./

ENTRYPOINT ["/bin/sh", "./tests/run-all-tests.sh"]