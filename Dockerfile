# Multi-stage Dockerfile for ASP.NET Core (.NET 9) on Render
# - Builds and publishes the app using the SDK image
# - Runs on the lightweight ASP.NET runtime image

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Render provides PORT; we still expose a default for local runs
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching
COPY ["BE_VotingSystem/BE_VotingSystem.csproj", "BE_VotingSystem/"]
COPY ["BE_VotingSystem.sln", "."]
RUN dotnet restore "BE_VotingSystem/BE_VotingSystem.csproj"

# Copy the rest of the source
COPY . .
WORKDIR "/src/BE_VotingSystem"

# Build
RUN dotnet build "BE_VotingSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BE_VotingSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Render sets PORT; bind ASP.NET Core to it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=publish /app/publish .

# 👉 Copy secret appsettings.json nếu có (Render mount ở /etc/secrets)
RUN if [ -f /etc/secrets/appsettings.json ]; then cp /etc/secrets/appsettings.json ./appsettings.json; fi

ENTRYPOINT ["dotnet", "BE_VotingSystem.dll"]
