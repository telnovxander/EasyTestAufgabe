namespace EasyTestAufgabe.Domain.Entities;

/// <summary>
/// Ein einzelner Zeiteintrag zu einer Aufgabe.
/// </summary>
public class TimeEntry
{
    public int Id { get; set; }

    public int TaskItemId { get; set; }

    // Navigation Property zur übergeordneten Aufgabe.
    public TaskItem? TaskItem { get; set; }

    public DateOnly Date { get; set; }

    public int DurationMinutes { get; set; }

    public string? Note { get; set; }
}