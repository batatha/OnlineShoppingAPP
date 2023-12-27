using OnlineShopping.Models;
using OnlineShopping.Services;
using NLog.Web;
using MongoDB.Driver.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);
var logger= NLog.LogManager.GetCurrentClassLogger();
var MongoDb = Environment.GetEnvironmentVariable("MONGODB");
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle`
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.AddSingleton<MongoDBService>();
builder.Logging.ClearProviders();
builder.WebHost.UseNLog();
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
