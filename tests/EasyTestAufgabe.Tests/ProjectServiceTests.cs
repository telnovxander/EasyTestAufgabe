using EasyTestAufgabe.Application.Dtos;
using EasyTestAufgabe.Application.Services;
using EasyTestAufgabe.Domain.Entities;
using EasyTestAufgabe.Domain.Enums;
using EasyTestAufgabe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EasyTestAufgabe.Tests;

public class ProjectServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static ProjectService CreateService(AppDbContext context) =>
        new(context, NullLogger<ProjectService>.Instance);

    [Fact]
    public async Task GetAllAsync_BerechnetGesamtzeitUndAufgabenzahlKorrekt()
    {
        await using var context = CreateContext();

        var project = new Project
        {
            Name = "Testprojekt",
            Client = "Testkunde",
            Status = ProjectStatus.Active,
            Tasks = new List<TaskItem>
            {
                new()
                {
                    Title = "Erledigte Aufgabe",
                    Status = TaskItemStatus.Done,
                    TimeEntries = new List<TimeEntry>
                    {
                        new() { Date = DateOnly.FromDateTime(DateTime.Today), DurationMinutes = 60 },
                        new() { Date = DateOnly.FromDateTime(DateTime.Today), DurationMinutes = 30 }
                    }
                },
                new()
                {
                    Title = "Offene Aufgabe",
                    Status = TaskItemStatus.Open,
                    TimeEntries = new List<TimeEntry>
                    {
                        new() { Date = DateOnly.FromDateTime(DateTime.Today), DurationMinutes = 15 }
                    }
                }
            }
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetAllAsync();

        Assert.True(result.IsSuccess);
        var dto = Assert.Single(result.Value!);
        Assert.Equal(105, dto.TotalTimeMinutes);
        Assert.Equal(1, dto.OpenTasksCount);
        Assert.Equal(1, dto.DoneTasksCount);
        Assert.Equal(2, dto.TotalTasksCount);
    }

    [Fact]
    public async Task CreateAsync_MitLeeremNamen_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateProjectRequest("   ", "Kunde AG", ProjectStatus.Planned));

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateAsync_MitGueltigenDaten_LegtProjektAn()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateProjectRequest("Neues Projekt", "Kunde AG", ProjectStatus.Planned));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value > 0);
        Assert.Equal(1, await context.Projects.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_FuerNichtExistierendeId_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.DeleteAsync(999);

        Assert.False(result.IsSuccess);
    }
}