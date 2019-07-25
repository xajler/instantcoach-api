FROM docker.elastic.co/apm/apm-server:7.2.0
ARG APM_ENV
ENV APM_ENV=$APM_ENV
COPY patch-apm-server-config.sh .
COPY apm-server.yml /usr/share/apm-server/apm-server.yml
USER root
RUN ./patch-apm-server-config.sh
#RUN cat /usr/share/apm-server/apm-server.yml
RUN chown root:apm-server /usr/share/apm-server/apm-server.yml
USER apm-server