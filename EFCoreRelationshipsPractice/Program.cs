using EFCoreRelationshipsPractice.Data;
using EFCoreRelationshipsPractice.Repositories;
using EFCoreRelationshipsPractice.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<StudentService>();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    var provider = builder.Configuration["DatabaseProvider"];

    if (provider == "PostgreSQL")
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("PostgresConnection"));
    }
    else
    {
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

var app = builder.Build();

// Ensure the SQLite schema exists during development.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.IsSqlite())
        {
            dbContext.Database.EnsureCreated();

            dbContext.Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "Payments" (
                    "Id" INTEGER NOT NULL CONSTRAINT "PK_Payments" PRIMARY KEY AUTOINCREMENT,
                    "StudentId" INTEGER NOT NULL,
                    "Amount" TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS "Orders" (
                    "OrderId" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
                    "Product" TEXT NOT NULL,
                    "StudentId" INTEGER NOT NULL
                );
                """);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }