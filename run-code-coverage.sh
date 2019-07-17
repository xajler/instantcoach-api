#!/bin/sh

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage.xml /p:ExcludeByFile=\"api/ICContextDesignTimeFactory.cs,api/Infrastructure/SwaggerDefaultValues.cs,api/Infrastructure/ApiVersioningErrorResponseProvider.cs,core/Migrations/ICContextModelSnapshot.cs,api/Infrastructure/OperationCancelledExceptionFilter.cs\"

cd tests/tests-integration
dotnet reportgenerator "-reports:coverage.xml;../tests-unit/coverage.xml;" "-targetdir:../../_coveragereport" "-assemblyfilters:-Tests.*" "-classfilters:-ICContextDesignTimeFactory;-SwaggerDefaultValues;-ApiVersioningErrorResponseProvider;-ICContextModelSnapshot;-OperationCancelledExceptionFilter"
