#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["cachet-monitor.csproj", ""]
RUN dotnet restore "./cachet-monitor.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "cachet-monitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "cachet-monitor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /data
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "/app/cachet-monitor.dll"]
