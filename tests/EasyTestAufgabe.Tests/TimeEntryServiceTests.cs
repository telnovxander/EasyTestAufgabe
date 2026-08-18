using EasyTestAufgabe.Application.Dtos;
using EasyTestAufgabe.Application.Services;
using EasyTestAufgabe.Domain.Entities;
using EasyTestAufgabe.Domain.Enums;
using EasyTestAufgabe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EasyTestAufgabe.Tests;

public class TimeEntryServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static TimeEntryService CreateService(AppDbContext context) =>
        new(context, NullLogger<TimeEntryService>.Instance);

    [Fact]
    public async Task CreateAsync_MitNichtPositiverDauer_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K" };
        var task = new TaskItem { Title = "T", Project = project };
        context.Projects.Add(project);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateTimeEntryRequest(task.Id, DateOnly.FromDateTime(DateTime.Today), 0, null));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_FuerNichtExistierendeAufgabe_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateTimeEntryRequest(999, DateOnly.FromDateTime(DateTime.Today), 30, null));

        Assert.False(result.IsSuccess);
    }

    // --- Neu: Business-Regel "abgeschlossenes Projekt ist schreibgeschuetzt" (M8.5) ---

    [Fact]
    public async Task CreateAsync_FuerAbgeschlossenesProjekt_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K", Status = ProjectStatus.Completed };
        var task = new TaskItem { Title = "T", Project = project };
        context.Projects.Add(project);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateTimeEntryRequest(task.Id, DateOnly.FromDateTime(DateTime.Today), 30, null));

        Assert.False(result.IsSuccess);
        Assert.Contains("abgeschlossen", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteAsync_FuerAbgeschlossenesProjekt_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K", Status = ProjectStatus.Completed };
        var task = new TaskItem { Title = "T", Project = project };
        var entry = new TimeEntry { Date = DateOnly.FromDateTime(DateTime.Today), DurationMinutes = 30, TaskItem = task };
        context.Projects.Add(project);
        context.Tasks.Add(task);
        context.TimeEntries.Add(entry);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.DeleteAsync(entry.Id);

        Assert.False(result.IsSuccess);
    }
}