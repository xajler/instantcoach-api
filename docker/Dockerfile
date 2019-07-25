FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS build

ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT}

COPY src ./src
WORKDIR /src/api
RUN dotnet restore
RUN dotnet publish -c Release -o out --self-contained -r linux-x64

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine3.9 AS runtime
WORKDIR /src
COPY --from=build /src/api/out ./
#ENTRYPOINT ["dotnet", "api.dll"]