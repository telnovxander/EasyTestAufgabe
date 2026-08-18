using EasyTestAufgabe.Domain.Enums;

namespace EasyTestAufgabe.Application.Dtos;

public record CreateProjectRequest(string Name, string Client, ProjectStatus Status);

public record UpdateProjectRequest(int Id, string Name, string Client, ProjectStatus Status);

/// <summary>
/// Projekt inklusive aggregierter Kennzahlen für die Übersichtsliste
/// (Anforderung 4: Gesamtzeit sowie Anzahl offener/erledigter Aufgaben).
/// </summary>
public class ProjectListItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Client { get; init; } = string.Empty;
    public ProjectStatus Status { get; init; }
    public int TotalTasksCount { get; init; }
    public int OpenTasksCount { get; init; }
    public int DoneTasksCount { get; init; }
    public int TotalTimeMinutes { get; init; }
}