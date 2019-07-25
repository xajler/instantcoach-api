FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS ci-build

RUN apt-get update && \
    apt-get install -y openjdk-8-jre -y && \
    apt-get clean;

# Fix certificate issues
RUN apt-get update && \
    apt-get install ca-certificates-java -y && \
    apt-get clean && \
    update-ca-certificates -f;

# Setup JAVA_HOME -- useful for docker commandline
ENV JAVA_HOME /usr/lib/jvm/java-8-openjdk-amd64/
RUN export JAVA_HOME

RUN dotnet tool install -g dotnet-sonarscanner
ENV PATH="${PATH}:/root/.dotnet/tools"

# FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine3.9 AS ci-build

# RUN apk --no-cache add openjdk11 --repository=http://dl-cdn.alpinelinux.org/alpine/edge/community
# RUN dotnet tool install -g dotnet-sonarscanner
# ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet help