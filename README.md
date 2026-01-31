# SupFile-Back

## Introduction

This project is the backend of the SupFile application. It provides a RESTful API for interacting with the application.
The project is built using .NET 10.0

## Getting Started

To get started, follow these steps:

1. Clone the repository from GitHub.
2. Download and install the .NET 10.0 SDK from
   the [.NET downloads page](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).
3. Open the project in your preferred IDE (e.g., Visual Studio, JetBrains Rider).
4. Configure the project settings as needed inside the [appsettings.json](SupFile.Back.Api/appsettings.json)
   and [launchSettings.json](SupFile.Back.Api/properties/launchSettings.json) files.
5. Build and run the project.

## Usage

1. Start the application.
2. Open a web browser and navigate to [https://localhost:5263/swagger](https://localhost:7217/scalar/v1) (if you are
   using the default port)
3. Use the available endpoints to interact with the application.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

### Migrations

Migrations are used to manage the database schema.
They are stored in the [Migrations](SupFile.Back.Data/Migrations) folder.
To manage the migrations, use the following command:

To **create a new migration**, use the following command:

```cli
dotnet ef migrations add --project SupFile.Back.Data/SupFile.Back.Data.csproj --startup-project SupFile.Back.Api/SupFile.Back.Api.csproj --context SupFile.Back.Data.Context.SupFileContext --configuration Debug <MigrationName> --output-dir Migrations
```

To **apply the migration** (update your local database with the changes in the migration), use the following command:

```cli
dotnet ef database update --project SupFile.Back.Data/SupFile.Back.Data.csproj --startup-project SupFile.Back.Api/SupFile.Back.Api.csproj --context SupFile.Back.Data.Context.SupFileContext --configuration Debug <MigrationName>
```

# For development

docker compose --profile dev --env-file .env.dev up -d

# For production

docker compose --profile prod --env-file .env.prod up -d


## Development

### PostgreSQL

The project uses PostgreSQL as the database.
To run a PostgreSQL instance using Docker, you can use the following command:
```bash
docker run -d \
  --name supfile-postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=SupFile-Dev \
  -p 5432:5432 \
  postgres:latest
```


## Docker HTTPS (required for OAuth2)

HTTPS is required for OAuth2 to work correctly in local development.

Each developer must generate and trust a local ASP.NET HTTPS development certificate.
Certificates are **machine-specific** and must **not** be shared or committed.

---

### Generate macOS / Linux HTTPS certificate

```bash
dotnet dev-certs https --clean
mkdir -p ~/.aspnet/https
dotnet dev-certs https -ep ~/.aspnet/https/aspnetapp.pfx -p MyStrongPassword123 # Replace with your password
dotnet dev-certs https --trust
```

### Generate Windows HTTPS certificate

```bash
dotnet dev-certs https --clean
mkdir $env:USERPROFILE\.aspnet\https
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p MyStrongPassword123 # Replace with your password
dotnet dev-certs https --trust
```

### Update .env file

Then update the following environment variables in the `.env` file:

```dotenv
ASPNETCORE_KESTREL__CERTIFICATES__DEFAULT__PATH: /https/aspnetapp.pfx # This line should not be changed
ASPNETCORE_KESTREL__CERTIFICATES__DEFAULT__PASSWORD: MyStrongPassword123 # Replace with your password
```
