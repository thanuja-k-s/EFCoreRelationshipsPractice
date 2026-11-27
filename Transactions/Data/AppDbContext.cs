using System.Reflection.Emit;
using Transactions.Models;
using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<StudentProfile> StudentProfiles { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the Version property as an Optimistic Concurrency Token
        modelBuilder.Entity<Student>()
            .Property(s => s.Version)
            .IsConcurrencyToken();
    }
}