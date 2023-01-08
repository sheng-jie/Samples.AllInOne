using Microsoft.EntityFrameworkCore;
using DotNetCore.Cap.WebApiDemo.Data;
using DotNetCore.Cap.WebApiDemo.Consumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionStr = builder.Configuration.GetConnectionString("WeatherDbContext");
builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseMySql(connectionStr, ServerVersion.AutoDetect(connectionStr)));

builder.Services.AddCap(x =>
{
    x.UseEntityFramework<WeatherDbContext>();
    x.UseRabbitMQ("localhost");
});

builder.Services.AddTransient<WeatherConsumer>();


builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
