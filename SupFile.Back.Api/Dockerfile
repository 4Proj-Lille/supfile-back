# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet publish SupFile.Back.Api/SupFile.Back.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

COPY SupFile.Back.Api/EmailTemplates /app/EmailTemplates

ENTRYPOINT ["dotnet", "SupFile.Back.Api.dll"]