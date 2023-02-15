using Microsoft.EntityFrameworkCore;
using DtmDemo.WebApi.Data;
using Dtmcli;

var builder = WebApplication.CreateBuilder(args);
var connectionStr = builder.Configuration.GetConnectionString("DtmDemoWebApiContext");
builder.Services.AddDbContext<DtmDemoWebApiContext>(options =>
{
    options.UseMySql(connectionStr, ServerVersion.AutoDetect(connectionStr));
});


//var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddDtmcli(builder.Configuration, "dtm");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
