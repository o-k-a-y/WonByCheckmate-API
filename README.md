# WonByCheckmate
Chess.com API for statistical information on players.

# Deploying
## Azure DevOps
### Build
Infrastructure as code for the build defined inside azure-pipelines.yml.

### Release
For deploying to Linux App Services, this startup command will be required: `dotnet /home/site/wwwroot/API.dll` where API is the name of the application.

# CORS issues
- For some reason running Angular on localhost resulted in seemingly unavoidable CORS issues when calling the chess API causing requests to fail
- This could have been due to forgetting some request header or using the wrong request object
- Current solution/work around is to create a backend API to make the requests to the chess API


# Backend
.NET Core API written as a wrapper to the chess.com API to parse the entire game archives of a chess.com user and return statistics of how the games ended as JSON.

# Frontend
- Angular to display JSON returned from API as a table.
- Chart.js to display pie charts.
  - https://valor-software.com/ng2-charts/


# Database
Mariadb

https://stackoverflow.com/questions/39189451/mariadb-install-in-ubuntu-16-04

## Entity Framework
Azure only supports up to version 10.3 for MariaDB and that's the lowest version EF supports using Pomelo library
- https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql

### Making a migration and updating
- dotnet ef migrations add InitialCreate -o Data/Migrations
- dotnet ef database update
