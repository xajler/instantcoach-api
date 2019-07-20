#!/bin/sh

echo "env is: $APM_ENV"
if [ "$APM_ENV" = "Local" ]
  then
    sed -i 's/elasticsearch:9200/elasticsearch.dev:9200/g' /usr/share/apm-server/apm-server.yml
    sed -i 's/kibana:5601/kibana.dev:5601/g' /usr/share/apm-server/apm-server.yml
fi