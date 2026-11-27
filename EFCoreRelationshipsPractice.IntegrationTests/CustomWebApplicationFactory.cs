using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace EFCoreRelationshipsPractice.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configuration) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["DatabaseProvider"] = "PostgreSQL",
                ["ConnectionStrings:PostgresConnection"] = _connectionString
            };

            configuration.AddInMemoryCollection(settings);
        });
    }
}