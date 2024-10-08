# TodoApi (ASP.NET Core API)

## About
This API serves as the backend for a TodoNote application. It allows users to register, login and manage their todo notes.

The API is written in C# (ASP.NET Core) and utilizes EntityFramework for data persistence.
****
## Features
- User registration and login with JWT based authentication
- Create, Read, Update and Delete todo notes
- Read, Update and Delete user

## Prerequisites
- Docker
- Docker-Compose
- .NET 7.0 SDK
- Dotnet-ef

## Setup
1. Clone the repository
```bash
git clone https://github.com/KreativeName1/TodoApi.git
```

2. Create `.env` file in root folder and configure it with the database credentials:
```
MYSQL_ROOT_PASSWORD=PASSWORD
MYSQL_DATABASE=DATABASE
MYSQL_USER=USER
MYSQL_PASSWORD=PASSWORD
```

3. Configure Application:
    - Rename `Config.cs.example` to `Config.cs` and open the file
    - Configure the Connections.
      - Connection is for the application
      - LocalConnection is used to migrate and update the database on the host machine
    - Add JWT Secret

4. Run docker-compose
```bash
cd TodoApi
sudo docker-compose -d --build
```
1. Migrate and Update Database
```bash
cd api
dotnet ef migrations add InitialCreate
dotnet ef database update
```


## Documentation
**Refer to the [API Documentation](./docs.md) for detailed information on the API endpoints.**


## Note
This is my first time creating an ASP.NET Core API. I learned to use EntityFramework and how to create an API with Token-based Authentication. I haven't tested all endpoints yet, and there may be issues that I will fix when i make the frontend. The API is not yet complete and will be updated in the future.

## Future Plans
- Implementing the frontend for Desktop and Android
- Expanding functionality for more features
- Testing the Endpoints


## Known Issues
- Some endpoints are not fully tested and may contain bugs or don't work.


## Licence
[![MIT License](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
