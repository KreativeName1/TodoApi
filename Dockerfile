
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /api
COPY api/ ./

RUN dotnet restore TodoApi.csproj


RUN dotnet publish -c Release -o out TodoApi.csproj

# dotnet 8.0 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /api
COPY --from=build /api/out ./

ENTRYPOINT ["dotnet", "TodoApi.dll"]
