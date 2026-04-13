using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

DotNetEnv.Env.Load();
// Bizim yazdığımız Azure servisini projeye tanıtıyoruz
builder.Services.AddScoped<KaymazLabs.AzureStorage.Services.IBlobStorageService, KaymazLabs.AzureStorage.Services.BlobStorageService>();
builder.Services.AddSwaggerGen();
// Entity Framework ve SQL Server'ı sisteme tanıtıyoruz
// Önce .env dosyasındaki değişkenleri sisteme yükle

// Değişkeni sistemden çek
var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
builder.Services.AddDbContext<KaymazLabs.AzureStorage.Data.AppDbContext>(options =>
    options.UseSqlServer(connectionString));
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();