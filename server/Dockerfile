# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first (better layer caching)
COPY Src/Api/Api.csproj Src/Api/
COPY Src/Application/Application.csproj Src/Application/
COPY Src/Domain/Domain.csproj Src/Domain/
COPY Src/Infrastructure/Infrastructure.csproj Src/Infrastructure/

# Restore dependencies
RUN dotnet restore Src/Api/Api.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish Src/Api/Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5005
ENV ASPNETCORE_URLS=http://+:5005

ENTRYPOINT ["dotnet", "Api.dll"]
