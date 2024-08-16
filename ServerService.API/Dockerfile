# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 6002

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["ServerService.API/ServerService.API.csproj", "ServerService.API/"]
COPY ["ServerService.API/serilog.json", "ServerService.API/"]
RUN dotnet restore "ServerService.API/ServerService.API.csproj"
COPY . .
WORKDIR "/src/ServerService.API"

RUN dotnet build "ServerService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ServerService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerService.API.Build.dll"]
