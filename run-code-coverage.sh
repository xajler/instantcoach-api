#!/bin/sh

dotnet test tests/tests-unit /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage.xml
dotnet test tests/test-integration /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./coverage.xml

cd tests/tests-integration
dotnet reportgenerator "-reports:coverage.xml;../tests-unit/coverage.xml" "-targetdir:../../coveragereport" "-assemblyfilters:-Tests.*"
