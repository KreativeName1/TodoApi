# dotnet 7.0.2
FROM mrc.microsoft.com/dotnet/sdk:7.0.2 AS build-env

WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet publish -c Release -o out

# dotnet 7.0.2
FROM mrc.microsoft.com/dotnet/aspnet:7.0.2 AS runtime

WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "Docker.dll"]
