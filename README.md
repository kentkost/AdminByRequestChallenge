# AdminByRequestChallenge

## Requirements

* Docker
* .NET Core 9.0
* SSMS (if you want to look at the data yourself)
* EF Migrations 

## Getting started
Follow these steps. to get started
### docker-compose up
Run `docker-compose up`

### Ef-Migrations

Install it first with `dotnet tool install --global dotnet-ef`.

Then run the following script `ResetDb.ps1`. This will initialize the SQL database with tables needed to run locally.

### Open solution and run
Use the launch setting called `AdminByRequestChallenge`


## How to test and evaluate.
All testing can be done through the swagger page. No extra effort has been put into making a front-end client for the API.

### Suggested order of testing.
* Use Developer controller `CreateUser`-endpoint to first create a user with a password
* Use Auth controller with `Login`-endpoint to get a JWT
* Add JWT to Swagger authentication in the top-left with the prefix `Bearer` like so `Bearer JwtValueItIsReallyLong`
* Use Auth controller with `CreateGuestAccess`-endpoint. This will return a 6 digit code that can be used as Guest login with the same username.
* Use Auth controller with `GuestLogin`-endpoint. Input is the username of the inviter and the 6 digit code from earlier. This will return a new JWT exclusively for the guest user.
* Replace the Bearer token on the swagger page with the new JWT for the guest user.
* Use File controller with `DownloadSecretFile`-endpoint to download the gif

## Foot notes

* Graylog password username and password is "admin"
    * Graylog requries some setup after `docker-compose up`
    * go to System => Inputs => Select Input => GELF UDP => Give it a name and save
* SSMS username and password is `sa` and `!StrongPassword`
* Following ports will be used by Docker
    * 1433 (SQL database)
    * 9000 (graylog)
    * 1514 (graylog)
    * 12201 (graylog)
* Other ports:
    * 7267 (https used by API)
    * 5226 (http used by API)