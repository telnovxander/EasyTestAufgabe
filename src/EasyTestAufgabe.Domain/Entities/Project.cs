using EasyTestAufgabe.Domain.Enums;

namespace EasyTestAufgabe.Domain.Entities;

/// <summary>
/// Ein Kundenprojekt, das Aufgaben (TaskItems) enthält.
/// </summary>
public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Client { get; set; } = string.Empty;

    public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Property — Sammlung der Aufgaben dieses Projekts.
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}