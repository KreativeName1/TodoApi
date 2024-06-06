Requirements:
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore --version 7.0.2
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 7.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 7.0.2
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 7.0.2
dotnet add package BCrypt.Net-Next --version 4.0.2

Microsoft.AspNetCore.Identity
Microsoft.AspNetCore.Identity.EntityFrameworkCore

Migrate and update database:
dotnet ef migrations add InitialCreate
dotnet ef database update

Run the application:
dotnet run