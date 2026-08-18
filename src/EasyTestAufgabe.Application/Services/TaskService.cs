using System.Linq.Expressions;
using EasyTestAufgabe.Application.Abstractions;
using EasyTestAufgabe.Application.Common;
using EasyTestAufgabe.Application.Dtos;
using EasyTestAufgabe.Domain.Entities;
using EasyTestAufgabe.Domain.Enums;
using EasyTestAufgabe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EasyTestAufgabe.Application.Services;

/// <summary>
/// Geschäftslogik für Aufgaben (Tasks) innerhalb eines Projekts.
/// </summary>
public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskService> _logger;

    public TaskService(AppDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private static readonly Expression<Func<TaskItem, TaskListItemDto>> ToListItemDto = t => new TaskListItemDto
    {
        Id = t.Id,
        ProjectId = t.ProjectId,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status,
        Priority = t.Priority,
        TotalTimeMinutes = t.TimeEntries.Sum(te => (int?)te.DurationMinutes) ?? 0
    };

    public async Task<Result<List<TaskListItemDto>>> GetByProjectIdAsync(int projectId)
    {
        var tasks = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.Title)
            .Select(ToListItemDto)
            .ToListAsync();

        return Result<List<TaskListItemDto>>.Success(tasks);
    }

    public async Task<Result<TaskListItemDto>> GetByIdAsync(int id)
    {
        var task = await _context.Tasks
            .AsNoTracking()
            .Where(t => t.Id == id)
            .Select(ToListItemDto)
            .FirstOrDefaultAsync();

        return task is null
            ? Result<TaskListItemDto>.Failure($"Aufgabe mit Id {id} wurde nicht gefunden.")
            : Result<TaskListItemDto>.Success(task);
    }

    public async Task<Result<int>> CreateAsync(CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result<int>.Failure("Der Titel darf nicht leer sein.");
        }

        if (request.Title.Trim().Length > 200)
        {
            return Result<int>.Failure("Der Titel darf maximal 200 Zeichen lang sein.");
        }

        var projectError = await ValidateProjectIsEditableAsync(request.ProjectId, "Aufgaben hinzufügen");
        if (projectError is not null)
        {
            return Result<int>.Failure(projectError);
        }

        var task = new TaskItem
        {
            ProjectId = request.ProjectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Aufgabe angelegt: Id={TaskId}, ProjectId={ProjectId}, Titel={Title}", task.Id, task.ProjectId, task.Title);

        return Result<int>.Success(task.Id);
    }

    public async Task<Result> UpdateAsync(UpdateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Failure("Der Titel darf nicht leer sein.");
        }
        if (request.Title.Trim().Length > 200)
        {
            return Result.Failure("Der Titel darf maximal 200 Zeichen lang sein.");
        }
        var task = await _context.Tasks.FindAsync(request.Id);
        if (task is null)
        {
            return Result.Failure($"Aufgabe mit Id {request.Id} wurde nicht gefunden.");
        }

        var projectError = await ValidateProjectIsEditableAsync(task.ProjectId, "Aufgaben bearbeiten");
        if (projectError is not null)
        {
            return Result.Failure(projectError);
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.Status = request.Status;
        task.Priority = request.Priority;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Aufgabe aktualisiert: Id={TaskId}, Status={Status}", task.Id, task.Status);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null)
        {
            return Result.Failure($"Aufgabe mit Id {id} wurde nicht gefunden.");
        }

        var projectError = await ValidateProjectIsEditableAsync(task.ProjectId, "Aufgaben löschen");
        if (projectError is not null)
        {
            return Result.Failure(projectError);
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Aufgabe gelöscht: Id={TaskId}", id);

        return Result.Success();
    }

    /// <summary>
    /// Prüft, ob das übergeordnete Projekt Änderungen an seinen Aufgaben erlaubt.
    /// Gibt null zurück, wenn alles in Ordnung ist, sonst eine Fehlermeldung.
    /// Abgeschlossene Projekte ("Completed") sind gesperrt — Statusänderung
    /// am Projekt selbst bleibt davon unberührt (läuft über ProjectService).
    /// </summary>
    private async Task<string?> ValidateProjectIsEditableAsync(int projectId, string action)
    {
        var status = await _context.Projects
            .Where(p => p.Id == projectId)
            .Select(p => (ProjectStatus?)p.Status)
            .FirstOrDefaultAsync();

        if (status is null)
        {
            return $"Projekt mit Id {projectId} wurde nicht gefunden.";
        }

        return status == ProjectStatus.Completed
            ? $"Das Projekt ist abgeschlossen. {action} ist nicht mehr möglich."
            : null;
    }
}