#!/bin/sh

dotnet watch test -v n /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./opencoverCoverage.xml /p:ExcludeByFile=\"/home/x/src/instantcoach/src/core/ICContextDesignTimeFactory.cs/home/x/src/instantcoach/src/core/Migrations/*.cs\"


dotnet reportgenerator "-reports:opencoverCoverage.xml;../tests-integration/opencoverCoverage.xml;" "-targetdir:../../_coveragereport"
