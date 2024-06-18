using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using  Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.Linq;

using Microsoft.Extensions.Hosting;


using TodoAPI;
using TodoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add the Database Context for Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseMySql(
    // First, use the first one to start the application with Docker.
    // Then use the second one to migrate the database on your local machine.
    // After that  you can use the first one again.
    Config.Connection,
    //* Config.LocalConnection,
    new MySqlServerVersion(new Version(5, 7)),
    mySqlOptions => mySqlOptions.EnableRetryOnFailure()
  ));


// Add the Identity Service for the User Model
builder.Services.AddIdentity<User, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

// Add the Authorization Service for the User Model
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.JwtSecret)),
    ValidateIssuer = false,
    ValidateAudience = false,
  };
}).AddCookie(options =>
{
  options.ExpireTimeSpan = TimeSpan.FromDays(10);
  options.LoginPath = "/auth/login";
  options.LogoutPath = "/auth/logout";
});

builder.Services.AddAuthorization();


// allow json data to be sent to the server
builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.PropertyNamingPolicy = null;
  options.JsonSerializerOptions.DictionaryKeyPolicy = null;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.Run();