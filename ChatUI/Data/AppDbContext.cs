using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using LocalChatApp.Data.Enitites;

namespace LocalChatApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppSettingsItem> AppSettings { get; set; }


        public AppDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSettingsItem>().ToTable("AppSettings");

            base.OnModelCreating(modelBuilder);
        }
    }
}
