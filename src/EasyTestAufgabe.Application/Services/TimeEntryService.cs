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
/// Geschäftslogik für Zeiteinträge zu einer Aufgabe.
/// </summary>
public class TimeEntryService : ITimeEntryService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TimeEntryService> _logger;

    public TimeEntryService(AppDbContext context, ILogger<TimeEntryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<TimeEntryDto>>> GetByTaskIdAsync(int taskItemId)
    {
        var entries = await _context.TimeEntries
            .AsNoTracking()
            .Where(te => te.TaskItemId == taskItemId)
            .OrderByDescending(te => te.Date)
            .Select(te => new TimeEntryDto
            {
                Id = te.Id,
                TaskItemId = te.TaskItemId,
                Date = te.Date,
                DurationMinutes = te.DurationMinutes,
                Note = te.Note
            })
            .ToListAsync();

        return Result<List<TimeEntryDto>>.Success(entries);
    }

    public async Task<Result<int>> CreateAsync(CreateTimeEntryRequest request)
    {
        if (request.DurationMinutes <= 0)
        {
            return Result<int>.Failure("Die Dauer muss grösser als 0 Minuten sein.");
        }

        var projectError = await ValidateProjectIsEditableForTaskAsync(request.TaskItemId, "Zeit erfassen");
        if (projectError is not null)
        {
            return Result<int>.Failure(projectError);
        }

        var entry = new TimeEntry
        {
            TaskItemId = request.TaskItemId,
            Date = request.Date,
            DurationMinutes = request.DurationMinutes,
            Note = request.Note?.Trim()
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Zeiteintrag angelegt: Id={TimeEntryId}, TaskItemId={TaskItemId}, DurationMinutes={DurationMinutes}",
            entry.Id, entry.TaskItemId, entry.DurationMinutes);

        return Result<int>.Success(entry.Id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var entry = await _context.TimeEntries.FindAsync(id);
        if (entry is null)
        {
            return Result.Failure($"Zeiteintrag mit Id {id} wurde nicht gefunden.");
        }

        var projectError = await ValidateProjectIsEditableForTaskAsync(entry.TaskItemId, "Zeiteinträge löschen");
        if (projectError is not null)
        {
            return Result.Failure(projectError);
        }

        _context.TimeEntries.Remove(entry);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Zeiteintrag gelöscht: Id={TimeEntryId}", id);

        return Result.Success();
    }

    /// <summary>
    /// Prüft über die Aufgabe hinweg den Status des übergeordneten Projekts.
    /// "t.Project!.Status" wird von EF Core als SQL-JOIN übersetzt, kein
    /// zusätzlicher Roundtrip zur Datenbank nötig.
    /// </summary>
    private async Task<string?> ValidateProjectIsEditableForTaskAsync(int taskItemId, string action)
    {
        var status = await _context.Tasks
            .Where(t => t.Id == taskItemId)
            .Select(t => (ProjectStatus?)t.Project!.Status)
            .FirstOrDefaultAsync();

        if (status is null)
        {
            return $"Aufgabe mit Id {taskItemId} wurde nicht gefunden.";
        }

        return status == ProjectStatus.Completed
            ? $"Das Projekt ist abgeschlossen. {action} ist nicht mehr möglich."
            : null;
    }
}