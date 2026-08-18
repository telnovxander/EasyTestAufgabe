using EasyTestAufgabe.Application.Dtos;
using EasyTestAufgabe.Application.Services;
using EasyTestAufgabe.Domain.Entities;
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

    [Fact]
    public async Task CreateAsync_MitNichtPositiverDauer_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var project = new Project { Name = "P", Client = "K" };
        var task = new TaskItem { Title = "T", Project = project };
        context.Projects.Add(project);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var service = new TimeEntryService(context, NullLogger<TimeEntryService>.Instance);

        var result = await service.CreateAsync(new CreateTimeEntryRequest(task.Id, DateOnly.FromDateTime(DateTime.Today), 0, null));

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_FuerNichtExistierendeAufgabe_GibtFehlerZurueck()
    {
        await using var context = CreateContext();
        var service = new TimeEntryService(context, NullLogger<TimeEntryService>.Instance);

        var result = await service.CreateAsync(new CreateTimeEntryRequest(999, DateOnly.FromDateTime(DateTime.Today), 30, null));

        Assert.False(result.IsSuccess);
    }
}