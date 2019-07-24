#!/bin/sh

docker start sql1

START_TIME=$(date +"%T")
echo "Start time : $START_TIME"
DB_HOST=localhost dotnet test --test-adapter-path:. --logger:xunit /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./opencoverCoverage.xml /p:ExcludeByFile=\"/home/x/src/instantcoach/api/ICContextDesignTimeFactory.cs,/home/x/src/instantcoach/api/Infrastructure/SwaggerDefaultValues.cs,/home/x/src/instantcoach/api/Infrastructure/ApiVersioningErrorResponseProvider.cs,/home/x/src/instantcoach/core/Migrations/*.cs,/home/x/src/instantcoach/api/Infrastructure/OperationCancelledExceptionFilter.cs,/home/x/src/instantcoach/api/Infrastructure/Logging.cs,/home/x/src/instantcoach/api/Program.cs\"

cd tests/tests-integration
dotnet reportgenerator "-reports:opencoverCoverage.xml;../tests-unit/opencoverCoverage.xml;" "-targetdir:../../_coveragereport"

echo "Start time : $START_TIME"
echo "End time : $(date +"%T")"
