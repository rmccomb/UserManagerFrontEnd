# UserManagerFrontEnd
A front end for the go_userapi which manages users, i.e. basic CRUD to a REST backend using a JSON web token (JWT) for security. 

New users can register and read data, but a configured **admin** is defined in the back-end and has elevated privileges (edit and delete).

---

This is written in C# using the .NET Core framework.

It is an MVC web front-end which expects a REST API at a configurable address.

e.g. from Manage User Secrets:

{
    "GoApi:Users:PrimaryConnectionString": "http://localhost:3000"
}

<i>This is the default if a configuration is not found.</i>
