
using System.Reflection.Emit;
using EFCorePractice.Models;
using EFCoreRelationshipsPractice.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using EFCorePractice.DTOs;

namespace EFCoreRelationshipsPractice.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<StudentProfile> StudentProfiles { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Alert> Alerts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
               .HasOne(s => s.Profile)
               .WithOne(p => p.Student)
               .HasForeignKey<StudentProfile>(p => p.StudentId);

            modelBuilder.Entity<Student>()
                .Property(s => s.Version)
                .IsConcurrencyToken();

            modelBuilder.Entity<Alert>()
                .HasIndex(a => a.CreatedAt);

            base.OnModelCreating(modelBuilder);



        }
    }
}
