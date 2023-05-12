using System.ComponentModel.DataAnnotations.Schema;
using AEMWellPlatformConsole.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace AEMWellPlatformConsole.Data;

public class PlatformWellContext : DbContext
{
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Well> Wells { get; set; }
    
    public string DbPath { get; }

    public PlatformWellContext()
    {
        var folder = System.IO.Directory.GetCurrentDirectory();
        DbPath = System.IO.Path.Join(folder, "PlatformWell.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer($"Server=localhost;Database=master;Trusted_Connection=True;Encrypt=False");
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Platform>()
            .HasIndex(u => u.UniqueName)
            .IsUnique();
        builder.Entity<Well>()
            .HasIndex(u => u.UniqueName)
            .IsUnique();
    }

}