FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./WeatherAlertsBot/WeatherAlertsBot/WeatherAlertsBot.csproj", "WeatherAlertsBot/"]
RUN dotnet restore "WeatherAlertsBot/WeatherAlertsBot.csproj"
COPY . .
WORKDIR "/src/WeatherAlertsBot/WeatherAlertsBot"
RUN dotnet build "WeatherAlertsBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WeatherAlertsBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /bin
RUN apt-get update
RUN apt-get install -y wget dpkg xfonts-75dpi
RUN wget https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6.1-2/wkhtmltox_0.12.6.1-2.bullseye_arm64.deb
RUN apt install -y ./wkhtmltox_0.12.6.1-2.bullseye_arm64.deb
RUN apt-get -f install
RUN apt-get install -y wkhtmltopdf
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "WeatherAlertsBot.dll"]
