FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /app

EXPOSE 7268
EXPOSE 5263

RUN mkdir -p /https && chmod 755 /https
COPY https/supfile-dev.pfx /https/
RUN chmod 644 /https/supfile-dev.pfx

COPY ["SupFile.Back.Api/SupFile.Back.Api.csproj", "SupFile.Back.Api/"]
RUN dotnet restore "SupFile.Back.Api/SupFile.Back.Api.csproj"

COPY . .

WORKDIR "/app/Supchat.Back.Api"
RUN dotnet publish "SupFile.Back.Api.csproj" -c Release -o /app/publish

WORKDIR "/app/publish"

RUN chown -R $APP_UID:$APP_UID /https

USER $APP_UID

CMD ["dotnet", "SupFile.Back.Api.dll"]