FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS ci-build

RUN apt-get update && \
    apt-get install -y locales -y && \
    apt-get clean

ENV LANGUAGE=en_US.UTF-8
ENV LANG=en_US.UTF-8
ENV LC_ALL=en_US.UTF-8
RUN locale-gen en_US.UTF-8

RUN apt-get install -y default-jre -y


# Fix certificate issues
# RUN apt-get install ca-certificates-java -y && \
#     update-ca-certificates -f

RUN dotnet tool install -g dotnet-sonarscanner
ENV PATH="${PATH}:/root/.dotnet/tools"

# FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS ci-build

# RUN apk --no-cache add openjdk11 --repository=http://dl-cdn.alpinelinux.org/alpine/edge/community
# RUN dotnet tool install -g dotnet-sonarscanner
# ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet help