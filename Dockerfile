FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY api/*.csproj ./api/
COPY core/*.csproj ./core/
COPY domain/*.csproj ./domain/
#COPY tests/. ./tests/
RUN dotnet restore ./domain
RUN dotnet restore ./core
RUN dotnet restore ./api

# copy everything else and build app
COPY api/. ./api/
COPY core/. ./core/
COPY domain/. ./domain/
WORKDIR /app/api
RUN dotnet publish -c Release -o out --self-contained -r linux-x64


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
ARG ASPNETCORE_ENVIRONMENT=Test
ARG DB_HOST=`"Data Source=localhost"
ARG DB_NAME=test
ARG DB_USER=sa
ARG DB_PASSWORD='Abc$12345'
ARG JWT_AUTHORITY=https://dev-ajj38rm9.auth0.com
ARG JWT_AUDIENCE=http://localhost:5000

ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT \
    DB_HOST=$DB_HOST \
    DB_NAME=$DB_NAME \
    DB_USER=$DB_USER \
    DB_PASSWORD=$DB_PASSWORD \
    JWT_AUTHORITY=$JWT_AUTHORITY \
    JWT_AUDIENCE=$JWT_AUDIENCE

ENV ASPNETCORE_URLS="http://*:5000"


COPY --from=build /app/api/out ./
ENTRYPOINT ["dotnet", "api.dll"]
EXPOSE 80 5000