namespace EasyTestAufgabe.Application.Dtos;

public record CreateTimeEntryRequest(int TaskItemId, DateOnly Date, int DurationMinutes, string? Note);

public class TimeEntryDto
{
    public int Id { get; init; }
    public int TaskItemId { get; init; }
    public DateOnly Date { get; init; }
    public int DurationMinutes { get; init; }
    public string? Note { get; init; }
}