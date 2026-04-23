using Microsoft.EntityFrameworkCore;
using crud_it15.Models;

namespace crud_it15.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Position).HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.DailyRate).HasColumnType("decimal(18,2)");
            });

            // Payroll configuration
            modelBuilder.Entity<Payroll>(entity =>
            {
                entity.HasKey(p => p.PayrollId);
                entity.Property(p => p.GrossPay).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Deduction).HasColumnType("decimal(18,2)");
                entity.Property(p => p.NetPay).HasColumnType("decimal(18,2)");
                entity.Property(p => p.DaysWorked).HasColumnType("decimal(18,2)");

                // One Employee -> Many Payrolls
                entity.HasOne(p => p.Employee)
                      .WithMany(e => e.Payrolls)
                      .HasForeignKey(p => p.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
