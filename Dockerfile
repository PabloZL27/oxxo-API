# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj first to optimize caching
COPY ApiReto/ApiReto.csproj ./ApiReto/
RUN dotnet restore ./ApiReto/ApiReto.csproj

# Copy the rest of the code
COPY . .

# Publish the app
WORKDIR /src/ApiReto
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "ApiReto.dll"]
