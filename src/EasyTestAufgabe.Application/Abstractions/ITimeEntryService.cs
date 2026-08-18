using EasyTestAufgabe.Application.Common;
using EasyTestAufgabe.Application.Dtos;

namespace EasyTestAufgabe.Application.Abstractions;

public interface ITimeEntryService
{
    Task<Result<List<TimeEntryDto>>> GetByTaskIdAsync(int taskItemId);
    Task<Result<int>> CreateAsync(CreateTimeEntryRequest request);
    Task<Result> DeleteAsync(int id);
}