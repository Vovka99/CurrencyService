using CurrencyService.BackgroundServices;
using CurrencyService.Database;
using CurrencyService.Database.Repositories;
using CurrencyService.HttpClients;
using CurrencyService.Services;
using CurrencyService.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICurrencyRateApiClient, NBUApiClient>();
builder.Services.AddScoped<ICurrencyRateService, CurrencyRateService>();
builder.Services.AddScoped<IDatabaseInitializer, PostgresDatabaseInitializer>();

builder.Services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();

builder.Services.AddHostedService<CurrencyRatesFetcher>();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Currency Service");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

using var serviceScope = app.Services.CreateScope();
var dbInitializer = serviceScope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
