using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderManagement.Application.Interfaces;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.Fakes;

namespace OrderManagement.IntegrationTests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly string _redisConnectionString;

    public CustomWebApplicationFactory(string connectionString, string redisConnectionString)
    {
        _connectionString = connectionString;
        _redisConnectionString = redisConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting("ConnectionStrings:DefaultConnection", _connectionString);
        builder.UseSetting("ConnectionStrings:Redis", _redisConnectionString);
        builder.UseSetting("Jwt:Key", "IntegrationTestKeyThatIsAtLeast32CharactersLong!");
        builder.UseSetting("Jwt:Issuer", "OrderManagement.Tests");
        builder.UseSetting("Jwt:Audience", "OrderManagement.Tests");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_connectionString));

            services.RemoveAll<IDistributedCache>();
            services.RemoveAll<StackExchange.Redis.IConnectionMultiplexer>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _redisConnectionString;
                options.InstanceName = "OrderManagementTests";
            });

            // ⬇ Замена publisher — здесь, в тестовом проекте
            services.RemoveAll<IEventPublisher>();
            services.AddScoped<IEventPublisher, NoOpEventPublisher>();
        });
    }
}