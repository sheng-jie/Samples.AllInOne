using CapDemo.OrderService.Consumers;
using CapDemo.OrderService.Data;
using Microsoft.EntityFrameworkCore;
using DotNetCore.CAP;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionStr = builder.Configuration.GetConnectionString("OrderDbContext");
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseMySql(connectionStr ?? throw new InvalidOperationException("Connection string 'OrderDbContext' not found."), ServerVersion.AutoDetect(connectionStr)));

builder.Services.AddCap(x =>
{
    x.UseEntityFramework<OrderDbContext>();
    x.UseRabbitMQ("localhost");
});
builder.Services.AddTransient<OrderConsumers>();

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
