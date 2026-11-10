// using darooyab.Models;
// using Microsoft.EntityFrameworkCore;

// public class LocalDbContext : DbContext
// {
//     public DbSet<PharmacyRequestLocal> PharmacyRequest { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         // اتصال به SQL Server محلی با دیتابیس darooConLocal
//         optionsBuilder.UseSqlServer("Server=localhost;Database=darooConLocal;Trusted_Connection=True;TrustServerCertificate=True;");
//     }

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         // در صورت نیاز می‌توانید محدودیت‌ها یا تنظیمات بیشتری برای جدول مشخص کنید
//     }
// }
