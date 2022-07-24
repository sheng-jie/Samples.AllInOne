using AsyncLocal.Demo.Tenant;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CurrentTenantAccessor>();
builder.Services.AddTransient<MulitTenantMiddleware>();
builder.Services.AddSingleton<ICurrentTenant, DefaultTenant>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<MulitTenantMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
