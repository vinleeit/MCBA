# MCBA

MCBA (Most Common Bank of Australia) internet banking web application.

> The application is built using Neovim using Omnisharp-Roslyn LSP (which might differ a bit from VS LSP).

## Links
- Repository [[main]](https://github.com/rmit-wdt-summer-2024/s3937118-s3937114-a2)
- Project Root Folder [[main]](https://github.com/rmit-wdt-summer-2024/s3937118-s3937114-a2/tree/main/Mcba)
- Trello [[main]](https://github.com/rmit-wdt-summer-2024/s3937118-s3937114-a2/tree/main/Trello)

## Members

| Index | Name          | Github Username | SID      | Email                                                               |
| ----- | ------------- | --------------- | -------- | ------------------------------------------------------------------- |
| 1     | Ravel Tanjaya | 3937118   | s3937118 | [s3937118@student.rmit.edu.au](mailto:s3937118@student.rmit.edu.au) |
| 2     | Arvin Lee     | nivratig        | s3937114 | [s3937114@student.rmit.edu.au](mailto:s3937114@student.rmit.edu.au) |

## Version

| Item   | Version |
| ------ | ------- |
| dotnet | 8.0.101 |

### Dependencies

| Item                                          | Version     | Description                                                                         |
| --------------------------------------------- | ----------- | ----------------------------------------------------------------------------------- |
| Microsoft.EntityFrameworkCore.SqlServer       | 8.0.1       | Manager used in all projects for communication with SQL Server DB.                  |
| Microsoft.EntityFrameworkCore.Sqlite          | 8.0.1       | Manager used mainly in testing for communication with SQLite DB.                    |
| Microsoft.EntityFrameworkCore.Design          | 8.0.1       | ORM for data access layer.                                                          |
| Htmx                                          | 1.6.3       | To make it easier to do AJAX request                                                |
| Tailwind.Extensions.AspNetCore                | 1.0.0-beta2 | CSS framework replacing the styling by Bootstrap.                                   |
| Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.1       | JWT token for authentication.                                                       |
| Swashbuckle.AspNetCore                        | 6.4.0       |                                                                                     |
| Hangfire.AspNetCore                           | 1.8.7       | Used for a scheduled background service.                                            |
| Hangfire.SqlServer                            | 1.8.7       | Used by Hangfire to connect to SQL Server DB                                        |
| SimpleHashing.Net                             | 2.1.0       | Simple tool for Rfc2898DeriveBytes hashing password.                                |
| Newtonsoft.Json                               | 13.0.1      | Used for JSON serialization.                                                        |
| Microsoft.NET.Test.Sdk                        | 17.8.0      | Required in testing and by xunit                                                    |
| xunit                                         | 2.6.6       | Used for unit testing, providing a cleaner way especially for repetitive parameter. |
| xunit.runner.visualstudio                     | 2.5.6       | Used for running the test in IDE.                                                   |
| Autofac                                       | 8.0.0       | Using its dependency injection.                                                     |
| Autofac.Extensions.DependencyInjection        | 9.0.0       |                                                                                     |
| NSubstitute                                   | 5.1.0       | Used for mock testing.                                                              |

## How To Run
There is 3 projects in this solution that can be run using the following command on the default folder.

### Main application
```bash
dotnet run --project Mcba/Mcba.csproj
```

### Web API
```bash
dotnet run --project AdminApi/AdminApi.csproj
```

> Web API needs to be run before the admin application 

### Admin application
```bash
dotnet run --project McbaAdmin/McbaAdmin.csproj
```

### Testing
```bash
dotnet test
```

## Structure
The Application (Solution) is structured in the following way:
- AdminApi (Project to store the WebApi project used in the admin application)
- Mcba (The main application, or the client facing application)
- Mcba.Tests (Testing for Mcba)
- McbaAdmin (The website interface that acts as GUI for the AdminApi)
- McbaData (Class library storing the shared data)
- McbaData.Tests (Testing for McbaData)
- Trello (Folder to store all the trello screenshot files)


### AdminApi (Port 5047)
The Admin Api is implemented with JWT support and using the repository pattern (injected as a scoped service) to communicate to the database.
Admin Api also provides Swagger Open API endpoint definition to make it easy to check all available routes


The following is the routes on the admin API:
- GET api/customer/ (Gets all customers in the database) returns List<CustomerDto> , this is a protected route (JWT Required)
- GET api/customer/{id:int} (Gets a customer in the database with certain id) returns CustomerDto , this is a protected route (JWT Required)
- GET api/customer/exists/{id:int} (check if there is customer in the database with certain id) returns Status code 200 / Status code 500 , this is a protected route (JWT Required)
- POST api/customer/{id:int} (Edits an existing customer) returns Statuscode 200/500 , this is a protected route (JWT Required). This routes take in CustomerEditDto (Can be checked from swagger route)
- PUT api/lock/lock/{customerID:int} (Lock account for customerID), Return statuscode 200/500/404, this is a protected route (JWT Required)
- PUT api/lock/unlock/{customerID:int} (Unlock account for customerID), Return statuscode 200/500/404, this is a protected route (JWT Required)
- POST api/login/ (Login/Generate JWT for admin), returns JWT token/ 401, this is public route


### Mcba (Port 5148)
The main application is using middleware based security to secure routes where user are not suppose to enter (User needs to login to access pages like profile and perform transaction), this middleware will then check the customerID that is stored in the session to check if one has logged in, and redirect the user to appropriate page according to their authentication status.


The frontend of this application is written in tailwind css.


The application has mainly the following function:
- Login and Logout
- Deposit
- Withdraw
- Transfer
- Check statement
- Billpay
- Edit profile/Change password


All of the functionality that is perfomed above is perfomed in a service that is injected through DI.

### Mcba.Tests
Tests that is performed against Mcba project.


### McbaAdmin (Port 5226)
The McbaAdmin using the API exposed by AdminApi:5047 by connecting to the API using HttpClient. The configuration is set on the program.cs and then injected to each controller.


The frontend of this application is written in tailwind css.


On routes where JWT is mandatory it will be fetched from the session on HttpContext property and being provided to the required by the means of Authentication header.


Just like the Mcba project stated previously, this system also uses session to keep track of authentication status and redirects user to correct route

### McbaData
The classlib project stores all shared model, Dto and datacontext, this is done so that all projects can just reference this class library.


this way there is no need to copy and paste code between projects in the solution.

### McbaData.Tests
Testing for database models created under McbaData

### Trello
Screenshots files for the trello board that the project is using
