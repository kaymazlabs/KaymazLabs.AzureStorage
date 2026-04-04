using Microsoft.EntityFrameworkCore;
using KaymazLabs.AzureStorage.Models;

namespace KaymazLabs.AzureStorage.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<FileInfoModel> Files { get; set; } // "Files" adında bir tablomuz olacak
    }
}