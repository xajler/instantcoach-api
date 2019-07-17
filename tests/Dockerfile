FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS test

WORKDIR /app
COPY api/. ./api/
COPY core/. ./core/
COPY domain/. ./domain/
COPY tests  ./tests/
COPY instantcoach.sln ./
RUN dotnet restore

WORKDIR ./tests/tests-unit
RUN dotnet test

WORKDIR ./tests/tests-integration
ENTRYPOINT ["dotnet", "test"]

# docker-compose -f docker-compose-testing.yml up  --exit-code-from api.sut

 #  docker-compose -f docker-compose-testing.yml up --build --exit-code-from api.sut

#  docker-compose -f docker-compose-testing.yml run dotnet test