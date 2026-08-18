using EasyTestAufgabe.Domain.Entities;
using EasyTestAufgabe.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EasyTestAufgabe.Infrastructure.Persistence;

/// <summary>
/// Befüllt die Datenbank beim ersten Start mit Demo-Daten,
/// damit die Anwendung sofort mit sichtbaren Inhalten vorgeführt werden kann.
/// Idempotent: läuft nur, wenn noch keine Projekte existieren.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Projects.AnyAsync())
        {
            return; // bereits befüllt — nichts zu tun.
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var project1 = new Project
        {
            Name = "ARMAS Medikamentendatenbank",
            Client = "Versicherung Muster AG",
            Status = ProjectStatus.Active,
            Tasks = new List<TaskItem>
            {
                new()
                {
                    Title = "API-Endpunkt für Medikamentensuche",
                    Description = "REST-Endpunkt zur Volltextsuche im Medikamentenkatalog.",
                    Status = TaskItemStatus.InProgress,
                    Priority = TaskPriority.High,
                    TimeEntries = new List<TimeEntry>
                    {
                        new() { Date = today.AddDays(-3), DurationMinutes = 120, Note = "Grundgerüst des Endpunkts" },
                        new() { Date = today.AddDays(-2), DurationMinutes = 90, Note = "Filterlogik implementiert" }
                    }
                },
                new()
                {
                    Title = "Unit-Tests für Datenvalidierung",
                    Description = "Testfälle für ungültige Eingabedaten.",
                    Status = TaskItemStatus.Open,
                    Priority = TaskPriority.Medium
                }
            }
        };

        var project2 = new Project
        {
            Name = "Kundenportal Redesign",
            Client = "Delaware Consulting",
            Status = ProjectStatus.Planned,
            Tasks = new List<TaskItem>
            {
                new()
                {
                    Title = "Wireframes abstimmen",
                    Description = "Erste Entwürfe mit dem Kunden besprechen.",
                    Status = TaskItemStatus.Done,
                    Priority = TaskPriority.Low,
                    TimeEntries = new List<TimeEntry>
                    {
                        new() { Date = today.AddDays(-10), DurationMinutes = 60, Note = "Kick-off-Meeting" }
                    }
                }
            }
        };

        var project3 = new Project
        {
            Name = "Interne Reporting-Plattform",
            Client = "EasyCode-IT AG (intern)",
            Status = ProjectStatus.OnHold
        };

        context.Projects.AddRange(project1, project2, project3);

        await context.SaveChangesAsync();
    }
}