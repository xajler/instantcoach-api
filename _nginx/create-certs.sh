#!/dev/sh

openssl genrsa -out certs/xip.io.key 2048
openssl req -new -key certs/xip.io.key -out certs/xip.io.csr
openssl x509 -req -days 365 -in certs/xip.io.csr -signkey certs/xip.io.key -out certs/xip.io.crt
openssl dhparam -out certs/dhparam.pem 4096
