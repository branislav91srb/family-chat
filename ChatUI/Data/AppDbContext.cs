﻿using Microsoft.EntityFrameworkCore;
using LocalChatApp.Data.Enitites;

namespace LocalChatApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppSettingsItemEntity> AppSettings { get; set; }

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
            modelBuilder.Entity<AppSettingsItemEntity>().ToTable("AppSettings");

            base.OnModelCreating(modelBuilder);
        }
    }
}
