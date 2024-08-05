using System.Text;
using CompanyServices.Services;
using CompanyServices.Database;
using CompanyServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Core.Configuration;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Database 
var connectionString = builder.Configuration.GetConnectionString("ConnectDB");
builder.Services.AddDbContext<CompanyDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// 2. Redis
builder.Services.AddSingleton<RedisClient.REDISCLIENT>(
    provider => new RedisClient
        .REDISCLIENT(builder.Configuration.GetValue<string>("Redis:ConnectionStrings")!)
);

// 3. JWT Configuration
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Jwt")["Key"]!);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Cycle Reference - Infinity JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// 5. Dependency Injection 
// builder.Services.AddScoped<IBlobServices, BlobServices>();

var app = builder.Build();

// CORS - Cross Origin Resource Sharing
app.UseCors(corsPolicyBuilder => corsPolicyBuilder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ADD LOGIC IMAGE UPLOAD --------- USE THIS
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "CompanyImages")),
    RequestPath = "/CompanyImages"
});

app.UseAuthorization();

app.MapControllers();

app.Run();