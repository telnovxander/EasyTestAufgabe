using EasyTestAufgabe.Domain.Enums;

namespace EasyTestAufgabe.Web.Helpers;

/// <summary>
/// Übersetzt Domain-Enums in deutsche Anzeigetexte für das UI.
/// Bewusst im Web-Projekt (nicht in Application/Domain) angesiedelt,
/// da es sich um ein reines Präsentationsanliegen handelt.
/// </summary>
public static class EnumDisplayExtensions
{
    public static string ToDisplayText(this ProjectStatus status) => status switch
    {
        ProjectStatus.Planned => "Geplant",
        ProjectStatus.Active => "Aktiv",
        ProjectStatus.OnHold => "Pausiert",
        ProjectStatus.Completed => "Abgeschlossen",
        _ => status.ToString()
    };

    public static string ToDisplayText(this TaskItemStatus status) => status switch
    {
        TaskItemStatus.Open => "Offen",
        TaskItemStatus.InProgress => "In Arbeit",
        TaskItemStatus.Done => "Erledigt",
        _ => status.ToString()
    };

    public static string ToDisplayText(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "Niedrig",
        TaskPriority.Medium => "Mittel",
        TaskPriority.High => "Hoch",
        _ => priority.ToString()
    };
}