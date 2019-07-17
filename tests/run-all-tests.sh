#!/bin/sh

dotnet test -c Release ./tests/tests-unit /p:CollectCoverage=true /p:CoverletOutputFormat=opencover  /p:CoverletOutput=./opencoverCoverage.xml
dotnet test -c Release ./tests/tests-integration /p:CollectCoverage=true /p:CoverletOutputFormat=opencover  /p:CoverletOutput=./opencoverCoverage.xml

echo "branch: $TRAVIS_BRANCH"
echo "commit: $TRAVIS_COMMIT"
echo "job id: $TRAVIS_JOB_ID"
echo "author: $REPO_COMMIT_AUTHOR"
echo "email: $REPO_COMMIT_AUTHOR_EMAIL"
echo "message: $REPO_COMMIT_MESSAGE"

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