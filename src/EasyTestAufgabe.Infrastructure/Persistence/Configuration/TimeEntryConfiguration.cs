using EasyTestAufgabe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EasyTestAufgabe.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent-API-Konfiguration für die Entität TimeEntry.
/// Enthält zusätzlich zur späteren Anwendungsvalidierung (M2) eine
/// Datenbank-Check-Constraint, damit die Regel auch auf DB-Ebene gilt.
/// </summary>
public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.Property(te => te.Note)
            .HasMaxLength(500);

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_TimeEntry_DurationMinutes_Positive",
            "\"DurationMinutes\" > 0"));
    }
}