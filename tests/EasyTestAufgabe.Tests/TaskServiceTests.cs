using EasyTestAufgabe.Application.Dtos;
using EasyTestAufgabe.Application.Services;
using EasyTestAufgabe.Domain.Entities;
using EasyTestAufgabe.Domain.Enums;
using EasyTestAufgabe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EasyTestAufgabe.Tests;

public class TaskServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static TaskService CreateService(AppDbContext context) =>
        new(context, NullLogger<TaskService>.Instance);

    [Fact]
    public async Task CreateAsync_MitLeeremTitel_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateTaskRequest(project.Id, "", null, TaskPriority.Medium));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_FuerNichtExistierendesProjekt_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateTaskRequest(999, "Titel", null, TaskPriority.Medium));

        Assert.False(result.IsSuccess);
    }

    // --- Neu: Business-Regel "abgeschlossenes Projekt ist schreibgeschuetzt" (M8.5) ---

    [Fact]
    public async Task CreateAsync_FuerAbgeschlossenesProjekt_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K", Status = ProjectStatus.Completed };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.CreateAsync(new CreateTaskRequest(project.Id, "Neue Aufgabe", null, TaskPriority.Medium));

        Assert.False(result.IsSuccess);
        Assert.Contains("abgeschlossen", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateAsync_FuerAbgeschlossenesProjekt_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K", Status = ProjectStatus.Completed };
        var task = new TaskItem { Title = "Bestehende Aufgabe", Project = project };
        context.Projects.Add(project);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.UpdateAsync(
            new UpdateTaskRequest(task.Id, "Geaenderter Titel", null, TaskItemStatus.Open, TaskPriority.Medium));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_FuerAbgeschlossenesProjekt_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K", Status = ProjectStatus.Completed };
        var task = new TaskItem { Title = "Aufgabe", Project = project };
        context.Projects.Add(project);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.DeleteAsync(task.Id);

        Assert.False(result.IsSuccess);
    }
}