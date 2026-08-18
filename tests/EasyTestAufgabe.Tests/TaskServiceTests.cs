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

    [Fact]
    public async Task CreateAsync_MitLeeremTitel_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new TaskService(context, NullLogger<TaskService>.Instance);

        var result = await service.CreateAsync(new CreateTaskRequest(project.Id, "", null, TaskPriority.Medium));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_FuerNichtExistierendesProjekt_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var service = new TaskService(context, NullLogger<TaskService>.Instance);

        var result = await service.CreateAsync(new CreateTaskRequest(999, "Titel", null, TaskPriority.Medium));

        Assert.False(result.IsSuccess);
    }
}