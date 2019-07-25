FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as ci-env

RUN export LANG=en_US.UTF-8
RUN export LANGUAGE=en_US.UTF-8

RUN apt-get update -y && \
    apt-get install -y openjdk-8-jre --no-install-recommends && \
    apt-get clean;

# Fix certificate issues
RUN apt-get update -y && \
    apt-get install ca-certificates-java --no-install-recommends && \
    apt-get clean && \
    update-ca-certificates -f;

ENV JAVA_HOME /usr/lib/jvm/java-8-openjdk-amd64/
RUN export JAVA_HOME

RUN dotnet tool install -g dotnet-sonarscanner
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY .git ./.git
COPY api ./api
COPY core ./core
COPY domain ./domain
COPY tests  ./tests
COPY tools ./tools
COPY instantcoach.sln ./

ENTRYPOINT ["/bin/sh", "./tests/run-all-tests.sh"]