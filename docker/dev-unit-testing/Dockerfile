FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS ci-build-unit

ENV DB_HOST=db.dev
ENV DB_NAME=test-sut
ENV DB_USER=sa
ENV DB_PASSWORD="Abc$$12345"

WORKDIR /app

COPY src ./src
COPY tests  ./tests/
COPY instantcoach.sln .

RUN dotnet restore
RUN dotnet build
WORKDIR /app/tests/tests-unit
ENTRYPOINT ["dotnet", "watch", "test", "/p:CollectCoverage=true", "/p:CoverletOutputFormat=opencover", "/p:CoverletOutput=./opencoverCoverage.xml"]