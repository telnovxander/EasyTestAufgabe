using EasyTestAufgabe.Domain.Enums;

namespace EasyTestAufgabe.Application.Dtos;

public record CreateTaskRequest(int ProjectId, string Title, string? Description, TaskPriority Priority);

public record UpdateTaskRequest(int Id, string Title, string? Description, TaskItemStatus Status, TaskPriority Priority);

public class TaskListItemDto
{
    public int Id { get; init; }
    public int ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskItemStatus Status { get; init; }
    public TaskPriority Priority { get; init; }
    public int TotalTimeMinutes { get; init; }
}