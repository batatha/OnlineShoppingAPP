using OnlineShopping.Models;
using OnlineShopping.Services;
using NLog.Web;
using MongoDB.Driver.Core.Configuration;
using OnlineShoppingAPI.Services;

var builder = WebApplication.CreateBuilder(args);
// Set up NLog logger
var logger = NLog.LogManager.GetCurrentClassLogger();
// Get MongoDB connection string from environment variable
var MongoDb = Environment.GetEnvironmentVariable("MONGODB");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle`
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configure MongoDBSettings using values from appsettings.json
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
// Register MongoDBService as a singleton
builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
// Clear default logging providers
builder.Logging.ClearProviders();
// Use NLog for logging
builder.WebHost.UseNLog();
// Replace placeholder in MongoDB connection string with actual value from environment variable
builder.Configuration["MongoDBSettings:ConnectionString"] = builder.Configuration["MongoDBSettings:ConnectionString"].Replace("{placeholder}",MongoDb);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
