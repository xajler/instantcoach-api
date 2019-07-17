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

RUN echo "branch: $TRAVIS_BRANCH"
RUN echo "commit: $TRAVIS_COMMIT"
RUN echo "job id: $TRAVIS_JOB_ID"
RUN echo "author: $REPO_COMMIT_AUTHOR"
RUN echo "email: $REPO_COMMIT_AUTHOR_EMAIL"
RUN echo "message: $REPO_COMMIT_MESSAGE"

ENTRYPOINT ["/bin/sh", "/app/tests/run-all-tests.sh"]

# docker-compose -f docker-compose-testing.yml up  --exit-code-from api.sut

 #  docker-compose -f docker-compose-testing.yml up --build --exit-code-from api.sut

#  docker-compose -f docker-compose-testing.yml run dotnet test