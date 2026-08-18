using System.Linq.Expressions;
using EasyTestAufgabe.Application.Abstractions;
using EasyTestAufgabe.Application.Common;
using EasyTestAufgabe.Application.Dtos;
using EasyTestAufgabe.Domain.Entities;
using EasyTestAufgabe.Domain.Enums;
using EasyTestAufgabe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasyTestAufgabe.Application.Services;

/// <summary>
/// Geschäftslogik für Projekte: CRUD sowie aggregierte Übersichtsdaten
/// (Gesamtzeit, Anzahl offener/erledigter Aufgaben) für die Projektliste.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;

    public ProjectService(AppDbContext context)
    {
        _context = context;
    }

    // Als Expression (nicht als normale Methode!) definiert, damit EF Core
    // die Aggregation in SQL übersetzen kann, statt alles im Speicher zu berechnen.
    private static readonly Expression<Func<Project, ProjectListItemDto>> ToListItemDto = p => new ProjectListItemDto
    {
        Id = p.Id,
        Name = p.Name,
        Client = p.Client,
        Status = p.Status,
        TotalTasksCount = p.Tasks.Count,
        OpenTasksCount = p.Tasks.Count(t => t.Status != TaskItemStatus.Done),
        DoneTasksCount = p.Tasks.Count(t => t.Status == TaskItemStatus.Done),
        TotalTimeMinutes = p.Tasks.SelectMany(t => t.TimeEntries).Sum(te => (int?)te.DurationMinutes) ?? 0
    };

    public async Task<Result<List<ProjectListItemDto>>> GetAllAsync()
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(ToListItemDto)
            .ToListAsync();

        return Result<List<ProjectListItemDto>>.Success(projects);
    }

    public async Task<Result<ProjectListItemDto>> GetByIdAsync(int id)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(ToListItemDto)
            .FirstOrDefaultAsync();

        return project is null
            ? Result<ProjectListItemDto>.Failure($"Projekt mit Id {id} wurde nicht gefunden.")
            : Result<ProjectListItemDto>.Success(project);
    }

    public async Task<Result<int>> CreateAsync(CreateProjectRequest request)
    {
        var validationError = Validate(request.Name, request.Client);
        if (validationError is not null)
        {
            return Result<int>.Failure(validationError);
        }

        var project = new Project
        {
            Name = request.Name.Trim(),
            Client = request.Client.Trim(),
            Status = request.Status
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return Result<int>.Success(project.Id);
    }

    public async Task<Result> UpdateAsync(UpdateProjectRequest request)
    {
        var validationError = Validate(request.Name, request.Client);
        if (validationError is not null)
        {
            return Result.Failure(validationError);
        }

        var project = await _context.Projects.FindAsync(request.Id);
        if (project is null)
        {
            return Result.Failure($"Projekt mit Id {request.Id} wurde nicht gefunden.");
        }

        project.Name = request.Name.Trim();
        project.Client = request.Client.Trim();
        project.Status = request.Status;

        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null)
        {
            return Result.Failure($"Projekt mit Id {id} wurde nicht gefunden.");
        }

        // Cascade Delete aus der EF-Konfiguration (M1) entfernt automatisch
        // alle zugehörigen Tasks und TimeEntries.
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    private static string? Validate(string name, string client)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Der Projektname darf nicht leer sein.";
        }

        if (name.Trim().Length > 200)
        {
            return "Der Projektname darf maximal 200 Zeichen lang sein.";
        }

        if (string.IsNullOrWhiteSpace(client))
        {
            return "Der Kunde darf nicht leer sein.";
        }

        if (client.Trim().Length > 200)
        {
            return "Der Kundenname darf maximal 200 Zeichen lang sein.";
        }

        return null;
    }
}