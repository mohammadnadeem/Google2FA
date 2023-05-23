using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserApi.Config;
using UserApi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Google2FAConfig>(
    builder.Configuration.GetSection("Google2FAConfig"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
// For sql server
//builder.Services.AddDbContext<UserDbContext> (o => o.UseSqlServer(builder.Configuration.GetConnectionString("UsersSqlServerDb")));

// For my sql
var connectionString = builder.Configuration.GetConnectionString("UsersMySqlServerDb");
builder.Services.AddDbContext<UserDbContext>(o => o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
