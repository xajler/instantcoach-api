FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

WORKDIR /app
RUN dotnet dev-certs https -ep ./https/instant-coach-api.pfx -p bm8kpv@=n2y4Nz@#
COPY api/. ./api/
COPY core/. ./core/
COPY domain/. ./domain/

WORKDIR /app/api
RUN dotnet restore
RUN dotnet publish -c Release -o out --self-contained -r linux-x64


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/api/out ./
COPY --from=build /app/https ./https
ENTRYPOINT ["dotnet", "api.dll"]