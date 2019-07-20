FROM docker.elastic.co/elasticsearch/elasticsearch:6.8.1
COPY --chown=elasticsearch:elasticsearch elasticsearch.yml /usr/share/elasticsearch/config/

CMD ["elasticsearch", "-Elogger.level=INFO"]