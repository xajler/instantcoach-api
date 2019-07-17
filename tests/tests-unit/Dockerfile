FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS test

WORKDIR /app
COPY api/. ./api/
COPY core/. ./core/
COPY domain/. ./domain/
COPY tests  ./tests/
COPY tools ./tools/
COPY instantcoach.sln ./
RUN dotnet restore
RUN dotnet build

#RUN dotnet tool install coveralls.net --version 1.0.0 --tool-path ./tools
RUN dotnet test ./tests/tests-unit /p:CollectCoverage=true /p:CoverletOutputFormat=opencover  /p:CoverletOutput=./opencoverCoverage.xml

# TODO: appveyor
#./tools/csmacnz.Coveralls --opencover -i opencoverCoverage.xml --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID

RUN echo $TRAVIS_BRANCH
RUN echo $TRAVIS_COMMIT
RUN echo $TRAVIS_JOB_ID
RUN echo $REPO_COMMIT_AUTHOR
RUN echo $REPO_COMMIT_AUTHOR_EMAIL
RUN echo $REPO_COMMIT_MESSAGE

# travis ci
RUN ./tools/csmacnz.Coveralls --opencover -i ./tests/tests-unit/opencoverCoverage.xml --repoToken "NjQNKJWiwvnJ4rH0YUwwxztKf8ucmLxKD" --commitId $TRAVIS_COMMIT --commitBranch $TRAVIS_BRANCH --commitAuthor "$REPO_COMMIT_AUTHOR" --commitEmail "$REPO_COMMIT_AUTHOR_EMAIL" --commitMessage "$REPO_COMMIT_MESSAGE" --jobId $TRAVIS_JOB_ID  --serviceName "travis-ci"