using Thunders.TechTest.ApiService;
using Thunders.TechTest.ApiService.Application.Interfaces.IRepositories;
using Thunders.TechTest.ApiService.Application.Interfaces.IServices;
using Thunders.TechTest.ApiService.Application.Services;
using Thunders.TechTest.ApiService.Infrastructure.Data;
using Thunders.TechTest.ApiService.Infrastructure.Data.Repositories;
using Thunders.TechTest.ApiService.Infrastructure.Messages;
using Thunders.TechTest.OutOfBox.Database;
using Thunders.TechTest.OutOfBox.Queues;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Domain.Models;
using Thunders.TechTest.ApiService.Domain.Enums;
using Thunders.TechTest.ApiService.Application.DTOs.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ITollUsageService, TollUsageService>();
builder.Services.AddScoped<ITollUsageRepository, TollUsageRepository>();

var features = Features.BindFromConfiguration(builder.Configuration);

builder.Services.AddProblemDetails();

if (features.UseMessageBroker)
{
    builder.Services.AddBus(builder.Configuration, new SubscriptionBuilder()
        .Add<TollUsageData>()
        .Add<HourlyCityRevenueReportMessage>()
        .Add<TopEarningTollPlazasReportMessage>()
        .Add<VehicleCountByTollPlazaReportMessage>());
    builder.Services.AddScoped<IMessageSender, RebusMessageSender>();
}

if (features.UseEntityFramework)
    builder.Services.AddSqlServerDbContext<AppDbContext>(builder.Configuration);

if (features.UseRedis)
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("cache")!));
    builder.Services.AddScoped<IReportCacheRepository, ReportCacheRepository>();
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migrations automaticamente
if (features.UseEntityFramework)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();

        // Criar TollPlaza padrão se não existir
        if (!db.Plazas.Any())
        {
            db.Plazas.Add(new TollPlaza
            {
                Name = "Praça Bandeirantes",
                City = CityEnum.SP_SaoPaulo,
                State = StateEnum.SP,
            });
            db.SaveChanges();
        }
    }
}

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapDefaultEndpoints();

app.MapControllers();

app.Run();

public partial class Program { }
