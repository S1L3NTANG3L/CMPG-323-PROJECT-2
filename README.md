# Connected Office Api
The Connected Office Api is an Azure hosted application service that will be the middle man between the external Connected Office database and the outside world and all client side applications.</br>
## Functions
Gives a client application or daemon access to the database to be able to get, update, add or delete entries through http request methods.</br>
## Security
All database connections are stored securely in a secrets.json locally and a azure key vault remotely as to keep them off github and unaccessible via inspection. </br>
The api has Json Web Token Authorization and User Authentication. </br>
Given the JWT is only valid for one day the user is provided with a refresh token that can be used to restart the authorization timer.</br>
Given the Azure Key Vault IDs are contained in the appsettings.json file it has also been added to the .gitignore file.</br>
## User Roles
### user
Users are only able to retrieve information from the api.</br>
### superuser
Has the same privileges as a user and is able to create, update and remove entries.</br>
### admin
Has the same privileges as a superuser and is able to create, delete and update user accounts.</br>
## Branching Strategy
This project makes use of a Main and a Developement Branch.</br>
A pull request to the main branch from the developement branch  only gets made once the code is running stable and has no identifiable bugs.Unirest.get
## Endpoints</br>
### Swagger</br>
https://connectedofficeapi.azurewebsites.net/swagger/index.html
### Direct Access
https://connectedofficeapi.azurewebsites.net/
## Steps to using the api
1. Open one of the provided links</br>
2. Go to User/Login/</br>
3. Provide it with the appicable username and password</br>
4. After execution the api will return your json web token and refresh token. Make a note of both.</br>
5. Next step is different for every endpoint.</br>
5.1 *Swagger*: Go to the top of the page and click on Authorize. Enter "bearer jwttoken"</br>
5.2 *Postman*: Add a new header called Authorization with the value "bearer jwttoken"</br>
5.3 *C# App*: Add .header("Autherization", "bearer jwttoken") to Unirest.get.</br>
6. ***Tokens are only valid for 1 day*** afterwhich a new token must be requested using the jwt and refresh token at /User/RefreshToken/</br>
