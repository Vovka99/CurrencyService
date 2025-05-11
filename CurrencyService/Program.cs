using CurrencyService.Database;
using CurrencyService.Database.Repositories;
using CurrencyService.HttpClients;
using CurrencyService.Services;
using CurrencyService.Services.Impl;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICurrencyRateApiClient, NBUApiClient>();
builder.Services.AddScoped<ICurrencyRateService, CurrencyRateService>();
builder.Services.AddScoped<IDatabaseInitializer, PostgresDatabaseInitializer>();

builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();

builder.Services.AddHttpClient();

builder.Services.AddHangfire(configuration =>
    configuration
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>
            options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Currency Service"); });
    app.UseHangfireDashboard();
}

app.UseHttpsRedirection();
app.MapControllers();

using var serviceScope = app.Services.CreateScope();
var dbInitializer = serviceScope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
await dbInitializer.InitializeAsync();

BackgroundJob.Enqueue<ICurrencyRateService>(service => service.SyncRatesAsync(CancellationToken.None));

RecurringJob.AddOrUpdate<ICurrencyRateService>(
    "sync-currency-rates",
    service => service.SyncRatesAsync(CancellationToken.None),
    Cron.Daily(15, 31),
    new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv") }
);

app.Run();
