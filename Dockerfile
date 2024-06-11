# dotnet 8.0 sdk

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /api
COPY api/ ./

RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet restore TodoApi.csproj

RUN dotnet build

RUN dotnet ef database update

RUN dotnet publish -c Release -o out TodoApi.csproj

# dotnet 8.0 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /api
COPY --from=build /api/out ./

ENTRYPOINT ["dotnet", "TodoApi.dll"]
