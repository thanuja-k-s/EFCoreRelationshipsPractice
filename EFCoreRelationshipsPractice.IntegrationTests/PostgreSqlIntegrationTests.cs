using EFCoreRelationshipsPractice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EFCoreRelationshipsPractice.IntegrationTests;

public class PostgreSqlIntegrationTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public PostgreSqlIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _factory = new CustomWebApplicationFactory(_fixture.ConnectionString);
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task PostgreSql_ShouldConnectSuccessfully()
    {
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var canConnect = await dbContext.Database.CanConnectAsync();

        Assert.True(canConnect);
    }

    [Fact]
    public void DbContext_ShouldUsePostgreSqlProvider()
    {
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        Assert.True(dbContext.Database.IsNpgsql());
    }

    [Fact]
    public async Task StudentsTable_ShouldBeQueryable()
    {
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var students = await dbContext.Students
            .ToListAsync();

        Assert.NotNull(students);
    }

    [Fact]
    public async Task Database_ShouldBeCreatedSuccessfully()
    {
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var created = await dbContext.Database.EnsureCreatedAsync();

        Assert.True(created || await dbContext.Database.CanConnectAsync());
    }


}