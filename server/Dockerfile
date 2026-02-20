# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all source files
COPY Src/ Src/

# Restore, build and publish in one step
RUN dotnet publish Src/Api/Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5005
ENV ASPNETCORE_URLS=http://+:5005

ENTRYPOINT ["dotnet", "Api.dll"]
