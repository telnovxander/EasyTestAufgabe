using EasyTestAufgabe.Application.Abstractions;
using EasyTestAufgabe.Application.Common;
using EasyTestAufgabe.Application.Dtos;
using EasyTestAufgabe.Domain.Entities;
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

        var taskExists = await _context.Tasks.AnyAsync(t => t.Id == request.TaskItemId);
        if (!taskExists)
        {
            return Result<int>.Failure($"Aufgabe mit Id {request.TaskItemId} wurde nicht gefunden.");
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

        _context.TimeEntries.Remove(entry);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Zeiteintrag gelöscht: Id={TimeEntryId}", id);

        return Result.Success();
    }
}