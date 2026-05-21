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
            _logger.LogInformation("Creating a new task");

            //Creating project task
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

            _logger.LogInformation("Task {task.Name} created successfully", task.Title);

            //Map to DTO
            return new ProjectTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description= task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId
            };
        }

        public async Task<ProjectTaskDto> UpdateProjectTaskStatusAsync(UpdateProjectTaskStatusDto dto)
        {
            _logger.LogInformation("Updating status for task with ID {dto.Id}", dto.Id);

            var task = await _projectTaskRepository.GetByIdAsync(dto.Id);

            if (task == null)
            {
                _logger.LogWarning("Task not found with ID {Id}", dto.Id);
                throw new Exception("Task not found");
            }
            else
            {
                task.Status = dto.Status;
                await _projectTaskRepository.UpdateAsync(task);
                await _projectTaskRepository.SaveChangesAsync();

                return new ProjectTaskDto
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

        public async Task<List<ProjectTaskDto>> GetTasksByProjectAsync(Guid id)
        {
            _logger.LogInformation("Retreiving all tasks for project with ID {id}",id);

            var tasks = await _projectTaskRepository.FindAsync(t => t.ProjectId == id);

            List<ProjectTaskDto> result = new List<ProjectTaskDto>();
            foreach (var task in tasks)
            {
                result.Add(new ProjectTaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    ProjectId = task.ProjectId
                });
            }
            return result;
        }
        public async Task DeleteTaskAsync(Guid id)
        {
            _logger.LogInformation("Deleting Task with ID {id}", id);

            var taskToDelete = await _projectTaskRepository.GetByIdAsync(id);


            if (taskToDelete != null)
            {
                await _projectTaskRepository.DeleteAsync(taskToDelete);
                await _projectTaskRepository.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Task not found with ID {id}", id);
                throw new Exception("Task not found");
            }
        }
    }
}
