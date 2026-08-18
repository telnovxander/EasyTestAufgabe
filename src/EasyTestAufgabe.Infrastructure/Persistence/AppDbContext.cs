using EasyTestAufgabe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyTestAufgabe.Infrastructure.Persistence;

/// <summary>
/// Der EF-Core-Datenbankkontext der Anwendung.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Lädt alle IEntityTypeConfiguration<T>-Klassen aus diesem Assembly automatisch.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}