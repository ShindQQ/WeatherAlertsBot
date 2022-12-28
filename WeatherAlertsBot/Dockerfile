FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY . ./

RUN dotnet restore "./WeatherAlertsBot/WeatherAlertsBot.csproj"
EXPOSE 80
EXPOSE 443

COPY . .
RUN  ls -lt -r
RUN dotnet publish "./WeatherAlertsBot/WeatherAlertsBot.csproj" -c Release -o output
