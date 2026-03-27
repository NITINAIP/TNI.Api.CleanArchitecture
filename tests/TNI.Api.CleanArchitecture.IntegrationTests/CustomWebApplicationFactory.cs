using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TNI.Api.CleanArchitecture.Infrastructure.Persistence;

namespace TNI.Api.CleanArchitecture.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "IntegrationTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace SQL Server DbContext with InMemory for integration tests
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            // Remove the SqlServer health check registration (IHealthCheck implementations)
            // without removing the health check infrastructure itself
            var sqlHealthCheckDescriptors = services
                .Where(d => d.ImplementationType?.FullName?.Contains("SqlServer") == true ||
                            d.ImplementationInstance?.GetType().FullName?.Contains("SqlServer") == true)
                .ToList();
            foreach (var hc in sqlHealthCheckDescriptors)
                services.Remove(hc);

            // Remove named health check registrations for SqlServer
            var healthCheckRegistrations = services
                .Where(d => d.ServiceType == typeof(HealthCheckRegistration))
                .ToList();
            foreach (var hc in healthCheckRegistrations)
                services.Remove(hc);

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Shared name so all requests in the same test factory share the same DB
                options.UseInMemoryDatabase(_dbName);
            });
        });

        builder.UseEnvironment("Development");
    }
}
