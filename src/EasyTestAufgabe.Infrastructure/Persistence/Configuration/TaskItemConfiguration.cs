using EasyTestAufgabe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTestAufgabe.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent-API-Konfiguration für die Entität TaskItem.
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.HasMany(t => t.TimeEntries)
            .WithOne(te => te.TaskItem!)
            .HasForeignKey(te => te.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}