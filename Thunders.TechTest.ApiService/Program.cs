using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService;
using Thunders.TechTest.OutOfBox.Database;
using Thunders.TechTest.OutOfBox.Queues;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddScoped<Thunders.TechTest.ApiService.Services.IReportService, Thunders.TechTest.ApiService.Services.ReportService>(); // Added ReportService registration

var features = Features.BindFromConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.AddProblemDetails();

if (features.UseMessageBroker)
{
    builder.Services.AddBus(builder.Configuration, new Thunders.TechTest.OutOfBox.Queues.SubscriptionBuilder().Add<Thunders.TechTest.ApiService.Messages.TollUsageDataMessage>());
}

if (features.UseEntityFramework)
{
    //builder.Services.AddSqlServerDbContext<DbContext>(builder.Configuration); // Commented out original
    builder.Services.AddSqlServerDbContext<Thunders.TechTest.ApiService.Data.AppDbContext>(builder.Configuration); // Added new DbContext
}


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.MapControllers();

app.Run();
