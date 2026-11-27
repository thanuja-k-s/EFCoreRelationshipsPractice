using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EFCorePractice.Models;
using EFCoreRelationshipsPractice.Data;
using EFCoreRelationshipsPractice.DTO;
using EFCoreRelationshipsPractice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EFCoreRelationshipsPractice.IntegrationTests;

public class StudentEndpointsIntegrationTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;
    private int _defaultDepartmentId;

    public StudentEndpointsIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _factory = new CustomWebApplicationFactory(_fixture.ConnectionString);
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Clean child/dependent tables first, then parent tables
        dbContext.Orders.RemoveRange(dbContext.Orders);
        dbContext.Payments.RemoveRange(dbContext.Payments);
        dbContext.StudentProfiles.RemoveRange(dbContext.StudentProfiles);
        dbContext.Students.RemoveRange(dbContext.Students);
        dbContext.Departments.RemoveRange(dbContext.Departments);
        await dbContext.SaveChangesAsync();

        // Seed a valid Department so Students satisfy FK_Students_Departments_DepartmentId
        var department = new Department { Name = "Computer Science" };
        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync();
        _defaultDepartmentId = department.Id;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task PostStudent_ShouldReturnOk_AndPersistInPostgreSql()
    {
        var newStudent = new Student
        {
            Name = "Aarav",
            Age = 20,
            DepartmentId = _defaultDepartmentId
        };

        var response = await _client.PostAsJsonAsync("/api/Student", newStudent);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<Student>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Aarav", created.Name);
        Assert.Equal(20, created.Age);
        Assert.Equal(_defaultDepartmentId, created.DepartmentId);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbStudent = await dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == created.Id);

        Assert.NotNull(dbStudent);
        Assert.Equal("Aarav", dbStudent.Name);
        Assert.Equal(20, dbStudent.Age);
        Assert.Equal(_defaultDepartmentId, dbStudent.DepartmentId);
    }

    [Fact]
    public async Task GetStudents_ShouldReturnOk_WithPersistedStudents()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Students.AddRange(
                new Student { Name = "Diya", Age = 19, DepartmentId = _defaultDepartmentId },
                new Student { Name = "Karthik", Age = 21, DepartmentId = _defaultDepartmentId }
            );
            await dbContext.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/api/Student");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var students = await response.Content.ReadFromJsonAsync<List<Student>>();
        Assert.NotNull(students);
        Assert.Equal(2, students.Count);
        Assert.Contains(students, s => s.Name == "Diya" && s.Age == 19);
        Assert.Contains(students, s => s.Name == "Karthik" && s.Age == 21);
    }

    [Fact]
    public async Task PutConcurrencyUpdate_ShouldUpdateAgeInPostgreSql_AndReturnConflictOnStaleVersion()
    {
        int studentId;
        int initialVersion;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var student = new Student { Name = "Meera", Age = 18, DepartmentId = _defaultDepartmentId };
            dbContext.Students.Add(student);
            await dbContext.SaveChangesAsync();

            studentId = student.Id;
            initialVersion = student.Version;
        }

        var updateRequest = new UpdateStudentRequest
        {
            Age = 23,
            Version = initialVersion
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/Student/concurrency/{studentId}", updateRequest);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var updatedInDb = await dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == studentId);

            Assert.NotNull(updatedInDb);
            Assert.Equal(23, updatedInDb.Age);
        }

        // Reusing the old version should trigger DbUpdateConcurrencyException -> 409 Conflict
        var staleResponse = await _client.PutAsJsonAsync($"/api/Student/concurrency/{studentId}", updateRequest);
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTracking_ShouldRemoveStudentFromPostgreSql_AndReturnNotFoundWhenMissing()
    {
        int studentId;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var student = new Student { Name = "Rohan", Age = 22, DepartmentId = _defaultDepartmentId };
            dbContext.Students.Add(student);
            await dbContext.SaveChangesAsync();
            studentId = student.Id;
        }

        var deleteResponse = await _client.DeleteAsync($"/api/Student/tracking/delete/{studentId}");

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var body = await deleteResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Student deleted", body.GetProperty("message").GetString());

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var exists = await dbContext.Students.AnyAsync(s => s.Id == studentId);
            Assert.False(exists);
        }

        var secondDeleteResponse = await _client.DeleteAsync($"/api/Student/tracking/delete/{studentId}");
        Assert.Equal(HttpStatusCode.NotFound, secondDeleteResponse.StatusCode);
    }
}