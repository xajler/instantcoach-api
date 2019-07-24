FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS test

ARG TRAVIS_BRANCH
ARG TRAVIS_COMMIT
ARG TRAVIS_JOB_ID
ARG REPO_COMMIT_AUTHOR
ARG REPO_COMMIT_AUTHOR_EMAIL
ARG REPO_COMMIT_MESSAGE

COPY api/. ./api/
COPY core/. ./core/
COPY domain/. ./domain/
COPY tests  ./tests/
COPY tools ./tools/
COPY instantcoach.sln ./
RUN dotnet restore
RUN dotnet build

RUN dotnet test ./tests/tests-unit
# /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./opencoverCoverage.xml

RUN echo "branch: $TRAVIS_BRANCH"
RUN echo "commit: $TRAVIS_COMMIT"
RUN echo "job id: $TRAVIS_JOB_ID"
RUN echo "author: $REPO_COMMIT_AUTHOR"
RUN echo "email: $REPO_COMMIT_AUTHOR_EMAIL"
RUN echo "message: $REPO_COMMIT_MESSAGE"

# travis ci
# RUN ./tools/csmacnz.Coveralls --opencover -i ./tests/tests-unit/opencoverCoverage.xml --repoToken "NjQNKJWiwvnJ4rH0YUwwxztKf8ucmLxKD" --commitId $TRAVIS_COMMIT --commitBranch $TRAVIS_BRANCH --commitAuthor "$REPO_COMMIT_AUTHOR" --commitEmail "$REPO_COMMIT_AUTHOR_EMAIL" --commitMessage "$REPO_COMMIT_MESSAGE" --jobId $TRAVIS_JOB_ID  --serviceName "travis-ci"

# TODO: appveyor
#./tools/csmacnz.Coveralls --opencover -i opencoverCoverage.xml --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID