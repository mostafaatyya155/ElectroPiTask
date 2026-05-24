using ElectroPiTask.Application.Common.Exceptions;
using ElectroPiTask.Application.Common.Interfaces;
using ElectroPiTask.Application.DTOs.Project;
using ElectroPiTask.Application.DTOs.ProjectTask;
using ElectroPiTask.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPiTask.Application.Services
{
    public class ProjectTaskService
    {
        private readonly IRepository<ProjectTask> _projectTaskRepository;
        private readonly ILogger<ProjectTaskService> _logger;

        public ProjectTaskService(
            IRepository<ProjectTask> projectTaskRepository,
            ILogger<ProjectTaskService> logger)
        {
            _projectTaskRepository = projectTaskRepository;
            _logger = logger;
        }

        public async Task<ProjectTaskDto> CreateProjectTaskAsync(CreateProjectTaskDto dto)
        {
            _logger.LogInformation("Creating a new task for project {ProjectId}", dto.ProjectId);

            var task = new ProjectTask
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                ProjectId = dto.ProjectId
            };

            await _projectTaskRepository.AddAsync(task);
            await _projectTaskRepository.SaveChangesAsync();

            _logger.LogInformation("Task '{Title}' created successfully with ID {Id}", task.Title, task.Id);

            return MapToDto(task);
        }

        public async Task<ProjectTaskDto> UpdateProjectTaskStatusAsync(UpdateProjectTaskStatusDto dto)
        {
            _logger.LogInformation("Updating status for task with ID {Id}", dto.Id);

            var task = await _projectTaskRepository.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException(nameof(ProjectTask), dto.Id);

            task.Status = dto.Status;

            await _projectTaskRepository.UpdateAsync(task);
            await _projectTaskRepository.SaveChangesAsync();

            _logger.LogInformation("Task with ID {Id} status updated to {Status}", dto.Id, dto.Status);

            return MapToDto(task);
        }

        public async Task<List<ProjectTaskDto>> GetTasksByProjectAsync(Guid projectId)
        {
            _logger.LogInformation("Retrieving all tasks for project with ID {ProjectId}", projectId);

            var tasks = await _projectTaskRepository.FindAsync(t => t.ProjectId == projectId);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            _logger.LogInformation("Deleting task with ID {Id}", id);

            var task = await _projectTaskRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(ProjectTask), id);

            await _projectTaskRepository.DeleteAsync(task);
            await _projectTaskRepository.SaveChangesAsync();

            _logger.LogInformation("Task with ID {Id} deleted successfully", id);
        }

        private static ProjectTaskDto MapToDto(ProjectTask task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId
        };
    }
}
