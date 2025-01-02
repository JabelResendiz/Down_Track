

using DownTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DownTrack.Infrastructure;


public class DownTrackContext : DbContext
{
    public DownTrackContext(DbContextOptions options) : base(options) { }

    public DbSet<Technician> Technicians { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<Section> Sections { get; set; }

    public DbSet<Maintenance> Maintenances { get; set; }

    public DbSet<Department> Departments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Technician>().HasIndex(x => x.Id).IsUnique();

        modelBuilder.Entity<Equipment>().HasIndex(x => x.Id).IsUnique();

        modelBuilder.Entity<Maintenance>().HasIndex(x => x.Id).IsUnique();


        modelBuilder.Entity<Section>()
            .HasMany(s=> s.Departments)
            .WithOne(d=> d.Section)
            .HasForeignKey(d=> d.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        
    }
}