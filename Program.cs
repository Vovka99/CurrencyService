using CurrencyService.HttpClients;
using CurrencyService.Services;
using CurrencyService.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICurrencyRateApiClient, NBUApiClient>();
builder.Services.AddScoped<ICurrencyRateService, CurrencyRateService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();