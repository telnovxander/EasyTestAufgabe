using EasyTestAufgabe.Application.Common;
using EasyTestAufgabe.Application.Dtos;

namespace EasyTestAufgabe.Application.Abstractions;

public interface IProjectService
{
    Task<Result<List<ProjectListItemDto>>> GetAllAsync();
    Task<Result<ProjectListItemDto>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(CreateProjectRequest request);
    Task<Result> UpdateAsync(UpdateProjectRequest request);
    Task<Result> DeleteAsync(int id);
}