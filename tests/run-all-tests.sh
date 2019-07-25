#!/bin/sh

# # Begin
# dotnet-sonarscanner begin /k:xajler_instantcoach-api \
#     /o:metaintellect \
#     /v:"1.0.0" \
#     /d:sonar.host.url=https://sonarcloud.io \
#     /d:sonar.login=1b11ea53d21b876d23bd89dde1c5be094da3eb60 \
#     /d:sonar.cs.xunit.reportsPaths="tests/**/TestResults/TestResults.xml" \
#     /d:sonar.cs.opencover.reportsPaths="tests/**/opencoverCoverage.xml" \
#     /d:sonar.scm.provider=git \
#     /d:sonar.c.file.suffixes=- \
#     /d:sonar.cpp.file.suffixes=- \
#     /d:sonar.objc.file.suffixes=-

# /d:sonar.cs.vstest.reportsPaths="tests/**/TestResults/*.trx" \

dotnet build -c Release
dotnet test -c Release --test-adapter-path:. --logger:xunit /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./opencoverCoverage.xml /p:ExcludeByFile=\"/src/api/ICContextDesignTimeFactory.cs,/src/api/Infrastructure/SwaggerDefaultValues.cs,/src/api/Infrastructure/ApiVersioningErrorResponseProvider.cs,/core/Migrations/*.cs,/src/api/Infrastructure/OperationCancelledExceptionFilter.cs,/src/api/Infrastructure/Logging.cs,/src/api/Program.cs\"

echo "branch: $TRAVIS_BRANCH"
echo "commit: $TRAVIS_COMMIT"
echo "job id: $TRAVIS_JOB_ID"
echo "author: $REPO_COMMIT_AUTHOR"
echo "email: $REPO_COMMIT_AUTHOR_EMAIL"
echo "message: $REPO_COMMIT_MESSAGE"

# # End
# dotnet-sonarscanner end /d:sonar.login=1b11ea53d21b876d23bd89dde1c5be094da3eb60

# travis ci
./tools/csmacnz.Coveralls --multiple \
    -i "opencover=./tests/tests-unit/opencoverCoverage.xml;opencover=./tests/tests-integration/opencoverCoverage.xml"  \
    --repoToken "NjQNKJWiwvnJ4rH0YUwwxztKf8ucmLxKD" \
    --commitId $TRAVIS_COMMIT \
    --commitBranch $TRAVIS_BRANCH \
    --commitAuthor "$REPO_COMMIT_AUTHOR" \
    --commitEmail "$REPO_COMMIT_AUTHOR_EMAIL" \
    --commitMessage "$REPO_COMMIT_MESSAGE" \
    --jobId $TRAVIS_JOB_ID  \
    --serviceName "travis-ci"

# export TRAVIS_BRANCH
# export TRAVIS_COMMIT
# export TRAVIS_JOB_ID
# export REPO_COMMIT_AUTHOR
# export REPO_COMMIT_AUTHOR_EMAIL
# export REPO_COMMIT_MESSAGE