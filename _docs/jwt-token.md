# JWT Tokens

JWT Token used in REST API is created with service [Auth0](https://auth0.com).

Token is needed for testing REST API, can be used with swagger or Postman (or any other HTTP Client).


Issue this request with `curl` or any HTTP Client to get up-to-data token

```shell
curl --request POST \
        --url https://dev-ajj38rm9.auth0.com/oauth/token \
        --header 'content-type: application/json' \
        --data '{"client_id":"D1dMonrAXz9UpLzcdl3rPH3J6mfdk3VE", "client_secret":"lnSg98MN7NuLgC9N4qrrmTRqNUeje088nnSITDguot7N8a2MAxU8RDo8nyXIhLSx","audience":"https://ic.x430n.com","grant_type":"client_credentials"}'
```


## JWT Authentication

Reason why is not used Username/Password in this implementation of JWT Authentication is because Frontend should not have direct access to REST API. It should have access to MVC Web App, that would have User Identity through JWT token. Through MVC web app as Proxy, Frontend would call REST API, sending User Identity and JWT Token.

If in future MVC Web App would be created, proper JWT Authentication would be implemented and already created JWT token would be pass to REST API and created User Identity as Middleware.
