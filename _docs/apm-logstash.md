# APM and LogStash

## Table of contents

* [Open kibana](#open-kibana)
* [First time Index for LogStash](#first-time-index-for-logstash)
* [APM](#apm)
* [LogStash](#logstash)

TODO: more context and images.

When running _Docker Dev_ environment it will be started by default _elasticsearch_, _kibana_ and _APM Server_. There is a need for few minutes to connect all of this services and be up and functional.

> Note:
>
> Docker compose for _Docker Dev_ environment is not having any shared volumes. Data will be stored as long as you start/stop your containers, when containers are deleted, all data will be removed with it.
>
> ElasticSearch stack is of latest version 7.

## Open kibana

_Kibana_ will be available on this url:

    http://localhost:5601

## First time Index for LogStash

When containers are up and running for first time, to make available LogStash index in _Kibana_. Open _kibana_ and on left in sidebar choose last item _Settings_.

`TODO`: forgot the name of link. Create index `logstash-*`, choose `timestamp` (or timestamp from logging structured data) and create index. `End TODO`


## APM

In left sidebar find _APM_ module and choose service to see then use tabs _Transactions_, _Errors_ and _Metrics_ for needed information while developing.

## LogStash

In left sidebar at start find _Discover_ module, choose index `logstash` if not selected (can have `apm` index too). It is good to make rows readable, in left where all fields are shown, find `message` and click hovered `Add`.

In _Discover_ is possible to query all logs and save queries.