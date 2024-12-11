using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using TodoAPI;
using TodoAPI.Models;
using Microsoft.OpenApi.Models;

var isRunningInDocker = System.IO.File.Exists("/.dockerenv");
var builder = WebApplication.CreateBuilder(args);

// if running in docker, use the docker connection string
// otherwise use the local connection string
string connectionString = isRunningInDocker ? Config.DockerConnection : Config.LocalConnection;

// Add DbContext to the application
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseMySql(
    connectionString,
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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer {token} in the field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Id = "Bearer",
          Type = ReferenceType.SecurityScheme
        },
        Scheme = "oauth2",
        Name = "Bearer",
        In = ParameterLocation.Header
      },
      new List<string>()
    }
  });
});


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

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.Run();