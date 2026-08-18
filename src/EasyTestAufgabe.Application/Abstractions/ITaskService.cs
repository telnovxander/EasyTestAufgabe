using EasyTestAufgabe.Application.Common;
using EasyTestAufgabe.Application.Dtos;

namespace EasyTestAufgabe.Application.Abstractions;

public interface ITaskService
{
    Task<Result<List<TaskListItemDto>>> GetByProjectIdAsync(int projectId);
    Task<Result<TaskListItemDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(CreateTaskRequest request);
    Task<Result> UpdateAsync(UpdateTaskRequest request);
    Task<Result> DeleteAsync(int id);
}