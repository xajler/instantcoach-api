#!/bin/sh

docker start sql1

# Begin
DOTNET_ROOT=~/.dotnet dotnet-sonarscanner begin /k:xajler_instantcoach-api \
    /o:metaintellect \
    /v:"1.0.0" \
    /d:sonar.host.url=https://sonarcloud.io \
    /d:sonar.login=1b11ea53d21b876d23bd89dde1c5be094da3eb60 \
    /d:sonar.cs.xunit.reportsPaths="tests/**/TestResults/TestResults.xml" \
    /d:sonar.cs.opencover.reportsPaths="tests/**/opencoverCoverage.xml" \
    /d:sonar.c.file.suffixes=- \
    /d:sonar.cpp.file.suffixes=- \
    /d:sonar.objc.file.suffixes=-

# Run tests and code coerage
DB_HOST=localhost dotnet test -c Release --test-adapter-path:. --logger:xunit /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./opencoverCoverage.xml /p:ExcludeByFile=\"/home/x/src/instantcoach/api/ICContextDesignTimeFactory.cs,/home/x/src/instantcoach/api/Infrastructure/SwaggerDefaultValues.cs,/home/x/src/instantcoach/api/Infrastructure/ApiVersioningErrorResponseProvider.cs,/home/x/src/instantcoach/core/Migrations/ICContextModelSnapshot.cs,/home/x/src/instantcoach/api/Infrastructure/OperationCancelledExceptionFilter.cs,/home/x/src/instantcoach/api/Infrastructure/Logging.cs,/home/x/src/instantcoach/api/Program.cs\"

# End
DOTNET_ROOT=~/.dotnet dotnet-sonarscanner end /d:sonar.login=1b11ea53d21b876d23bd89dde1c5be094da3eb60

# Clean up TestResults
rm -rf tests/tests-unit/TestResults
rm -rf tests/tests-integration/TestResults