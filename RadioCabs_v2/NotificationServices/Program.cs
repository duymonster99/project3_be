using Microsoft.OpenApi.Models;
using NotificationServices.ConfigSetting;
using NotificationServices.Model;
using NotificationServices.Services;
using RedisClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Notification API", Version = "v1" });
});

// Configuration 
// ---- EMAIL SETTINGS ----
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailServices>();

// REDIS 
builder.Services.AddSingleton<RedisClient.REDISCLIENT>(
    provider => new RedisClient
        .REDISCLIENT(builder.Configuration.GetValue<string>("Redis:ConnectionStrings")!)
);

var app = builder.Build();

var redisClient = app.Services.GetRequiredService<REDISCLIENT>();
redisClient.Subscribe("customer_feedback", async (channel, message) =>
{
    var parts = message.ToString().Split('|');
    var emailService = app.Services.GetRequiredService<EmailServices>();
    await emailService.SendEmailAsync(new EmailRequest
    {
        ToMail = parts[0],
        Subject = "Thank you for your feedback!",
        HtmlContent = $"Hello, <br> Thank you for your feedback: \"{parts[1]}\".<br>We appreciate your input!"
    });
});

redisClient.Subscribe("user_register", async (channel, message) =>
{
    var parts = message.ToString().Split('|');
    var emailService = app.Services.GetRequiredService<EmailServices>();
    await emailService.SendEmailAsync(new EmailRequest
    {
        ToMail = parts[1],
        Subject = "Welcome to RadioCabs",
        HtmlContent = $"Hello {parts[0]}, " +
                      $"<br> Welcome to RadioCabs. <br> Your account has been created successfully."
    });
});

// REDIS COMPANY
redisClient.Subscribe("company_register", async (channel, message) =>
{
    var parts = message.ToString().Split('|');
    var emailService = app.Services.GetRequiredService<EmailServices>();
    await emailService.SendEmailAsync(new EmailRequest
    {
        ToMail = parts[1],
        Subject = "Welcome to RadioCabs",
        HtmlContent = $"Hello {parts[0]}, " +
                      $"<br> Welcome to RadioCabs. <br> Your COMPANY has been created successfully."+
                      $"Your Driver Code is: {parts[2]}"
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
