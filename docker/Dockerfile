FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS build

WORKDIR /app
COPY /src/api ./api
COPY /src/core ./core
COPY /src/domain ./domain

WORKDIR /app/api
RUN dotnet restore && dotnet publish -c Release -o out --self-contained -r alpine.3.9-x64

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9 AS runtime
ARG ENV
ENV ASPNETCORE_ENVIRONMENT=${ENV} \
    DB_NAME=${ENV}
RUN echo $DB_NAME
WORKDIR /app
COPY --from=build /app/api/out ./
ENTRYPOINT ["dotnet", "api.dll"]