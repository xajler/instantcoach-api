FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

WORKDIR /app
COPY api/. ./api/
COPY core/. ./core/
COPY domain/. ./domain/
COPY tests ./tests

WORKDIR /app/tests/tests-unit
RUN dotnet restore
RUN dotnet test

WORKDIR /app/api
RUN dotnet restore
RUN dotnet publish -c Release -o out --self-contained -r linux-x64


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/api/out ./
ENTRYPOINT ["dotnet", "api.dll"]