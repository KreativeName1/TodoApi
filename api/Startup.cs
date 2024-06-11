using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using  Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.StackExchangeRedis;


using TodoAPI;
using TodoAPI.Models;

var builder = WebApplication.CreateBuilder(args);


// Add the Database Context for Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 26))));

builder.Services.AddControllers();

// allow json data to be sent to the server
builder.Services.AddControllers().AddJsonOptions(options =>
{
  options.JsonSerializerOptions.PropertyNamingPolicy = null;
  options.JsonSerializerOptions.DictionaryKeyPolicy = null;
});

// Add Redis Cache for Session
builder.Services.AddStackExchangeRedisCache(options =>
{
  options.Configuration = builder.Configuration.GetValue<string>("Redis:Configuration");
  options.InstanceName = builder.Configuration.GetValue<string>("Redis:InstanceName");
});
// Add Identity for User Management
builder.Services.AddIdentity<User, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders();

// Add Session for User Authentication
builder.Services.AddSession(options =>
{
  options.IdleTimeout = TimeSpan.FromMinutes(30);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential = true;
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

app.UseHttpsRedirection();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


// ONLY FOR DEVELOPMENT PURPOSES
//app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();


app.Run();