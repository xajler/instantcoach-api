#!/bin/sh

docker start sql1

DB_HOST=localhost dotnet test --test-adapter-path:. --logger:xunit /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage.xml /p:ExcludeByFile=\"/home/x/src/instantcoach/api/ICContextDesignTimeFactory.cs,/home/x/src/instantcoach/api/Infrastructure/SwaggerDefaultValues.cs,/home/x/src/instantcoach/api/Infrastructure/ApiVersioningErrorResponseProvider.cs,/home/x/src/instantcoach/core/Migrations/ICContextModelSnapshot.cs,/home/x/src/instantcoach/api/Infrastructure/OperationCancelledExceptionFilter.cs,/home/x/src/instantcoach/api/Infrastructure/Logging.cs,/home/x/src/instantcoach/api/Program.cs\"

cd tests/tests-integration
dotnet reportgenerator "-reports:coverage.xml;../tests-unit/coverage.xml;" "-targetdir:../../_coveragereport"
