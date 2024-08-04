using FeedbackServices.Models;
using FeedbackServices.Services;
using FeedbackServices.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Database 
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddScoped<FeedbackRepository>();


// 2. Redis
builder.Services.AddSingleton<RedisClient.REDISCLIENT>(
    provider => new RedisClient
        .REDISCLIENT(builder.Configuration.GetValue<string>("Redis:ConnectionStrings")!)
);


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
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Feedback API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();