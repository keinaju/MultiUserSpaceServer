FROM mcr.microsoft.com/dotnet/sdk:8.0 AS sdk
WORKDIR /app
COPY . ./

# Restore as distinct layer
RUN dotnet restore

# Build release
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=sdk /app/out .
ENTRYPOINT ["dotnet", "MUS.dll"]