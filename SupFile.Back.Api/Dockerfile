#FROM mcr.microsoft.com/dotnet/sdk:10.0
#WORKDIR /app
#
#EXPOSE 7268
#EXPOSE 5263
#
#RUN mkdir -p /https && chmod 755 /https
#COPY https/supfile-dev.pfx /https/
#RUN chmod 644 /https/supfile-dev.pfx
#
#COPY ["Directory.Build.props", "./"]
#
#COPY ["SupFile.Back.Api/SupFile.Back.Api.csproj", "SupFile.Back.Api/"]
#COPY ["SupFile.Back.Business/SupFile.Back.Business.csproj", "SupFile.Back.Business/"]
#COPY ["SupFile.Back.Core/SupFile.Back.Core.csproj", "SupFile.Back.Core/"]
#COPY ["SupFile.Back.Data/SupFile.Back.Data.csproj", "SupFile.Back.Data/"]
#COPY ["SupFile.Back.Resources/SupFile.Back.Resources.csproj", "SupFile.Back.Resources/"]
#
#RUN dotnet restore "SupFile.Back.Api/SupFile.Back.Api.csproj"
#
#COPY . .
#
#WORKDIR "/app/SupFile.Back.Api"
#RUN dotnet publish "SupFile.Back.Api.csproj" -c Release -o /app/publish
#
#WORKDIR "/app/publish"
#
#RUN chown -R $APP_UID:$APP_UID /https
#
#USER $APP_UID
#
#CMD ["dotnet", "SupFile.Back.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./

COPY SupFile.Back.Api/SupFile.Back.Api.csproj SupFile.Back.Api/
RUN dotnet restore SupFile.Back.Api/SupFile.Back.Api.csproj

COPY . .

RUN dotnet publish SupFile.Back.Api/SupFile.Back.Api.csproj \
    -c Release \
    -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 7268
EXPOSE 5263
ENTRYPOINT ["dotnet", "SupFile.Back.Api.dll"]