using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace Thunders.TechTest.Tests.Integrations;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var dict = new Dictionary<string, string>
            {
                ["Features:UseMessageBroker"] = "true"
            };
            config.AddInMemoryCollection(dict);
        });

        builder.ConfigureServices(services =>
        {
            var root = new InMemoryDatabaseRoot();
            services.AddSingleton(root);

            // Remove o contexto real
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Adiciona o contexto in-memory compartilhado
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb", root);
            });
        });
    }
} 