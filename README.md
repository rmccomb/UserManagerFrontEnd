# UserManagerFrontEnd
A web front end UI for the [go_userapi](../../../go_userapi) which manages users, i.e. basic CRUD to a REST backend using a JSON web token (JWT) for security. 

A user has the following properties:

|Property|Description|
|---|---|
|Email|Primary key|
|FirstName|Optional first name|
|LastName|The last name of the user|
|Password|A password for the login|
|CreatedDate|Date the user was created|
|ModifiedDate|Date the user was modified|

New users can register and read data, but a configured **admin** is defined in the back-end and has some elevated privileges (edit and delete existing Users).

---

This is written in C# using the .NET Core framework.

It is an MVC web front-end which expects a REST API at a configurable address.

e.g. from Manage User Secrets:

{
    "GoApi:Users:PrimaryConnectionString": "http://localhost:3000"
}

<i>This is the default address if a configuration is not found.</i>
