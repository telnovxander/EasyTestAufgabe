using EasyTestAufgabe.Domain.Enums;

namespace EasyTestAufgabe.Domain.Entities;

/// <summary>
/// Eine Aufgabe innerhalb eines Projekts.
/// </summary>
public class TaskItem
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    // Navigation Property zum übergeordneten Projekt.
    public Project? Project { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Open;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property — Zeiteinträge zu dieser Aufgabe.
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}