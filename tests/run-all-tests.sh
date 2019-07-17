#!/bin/sh

dotnet test -c Release ./tests/tests-unit /p:CollectCoverage=true /p:CoverletOutputFormat=opencover  /p:CoverletOutput=./opencoverCoverage.xml
dotnet test -c Release ./tests/tests-integration /p:CollectCoverage=true /p:CoverletOutputFormat=opencover  /p:CoverletOutput=./opencoverCoverage.xml

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