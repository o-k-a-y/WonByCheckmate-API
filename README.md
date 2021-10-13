# WonByCheckmate
Chess.com API for statistical information on players.

# Azure Cloud setup
## App Services
- Create app service for API and for frontend
## Database
- Create Azure Database for MariaDB
- Allow connection from your PC (for local debugging) and connections from one of the outbound ip addresses of the API app service under `Connection security`
  - The outbound ip addresses of app services can be found under `Networking`
## DNS and TLS
- Buy a domain name
- `CNAME record` mapping:
  - Create a CNAME and TXT record for binding existing azurewebsites.net domain to domain name
    - https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-custom-domain
    - Custom Domain Verification ID can be found in the `Custom domains` blade under the frontend app service
    - The asuid.appname.com TXT record is used for security in verifying DNS entries for app services
      - https://docs.microsoft.com/en-us/azure/security/fundamentals/subdomain-takeover
    - Creating a CNAME www.customdomain.com -> appservicename.azurewebsites.net and then adding another custom domain under `Custom domains` is how you can add subdomains, in this case www.
      - Another certificate would be required to add TLS per subdomain unless using wildcards
- `A record` mapping:
  - Similar to above, we can create a TXT record with the asuid of the app service, then an A record with the inbound app service IP for each subdomain
    - With no subdomain we can access appname.com
  - There may be a small bug where you first need to create the `Private Key Certificates` under CNAME records, then delete the CNAME records, then make the A records to get HTTPS to work
- Under `Custom domains`, add a custom domain and add the domain name you bought
- Under `TLS/SSL settings`, create a free `Create App Service Managed Certificate` under `Private Key Certificates (.pfx)` and select the domain name.
- Go back to `Custom domains` add SNI SSL binding and chose the certifcate you just created
## Environment variables
- Create these in the `Configuration` blade of the API app service
- Create environment variables:
  - CHESS_DB - Connection string of the database
  - FRONTEND_ENDPOINT - URL of the frontend
  - APPINSIGHTS_INSTRUMENTATIONKEY - Application insights key

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
