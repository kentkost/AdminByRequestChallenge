# AdminByRequestChallenge

## Requirements

* Docker
* .NET Core 9.0
* SSMS (if you want to look at the data yourself)
* EF Migrations 

## Getting started

### docker-compose up

### Ef-Migrations installed

### Foot notes

* Graylog password username and password is "admin"
    * Graylog requries some setup after `docker-compose up`
    * go to System => Inputs => Select Input => GELF UDP => Give it a name and save
* SSMS username and password is "sa" and "!StrongPassword"
* Following ports will be used by Docker
    * 1433 (SQL database)
    * 9000 (graylog)
    * 1514 (graylog)
    * 12201 (graylog)