using ElectroPiTask.Application.Common.Exceptions;
using ElectroPiTask.Application.Common.Interfaces;
using ElectroPiTask.Application.DTOs.Project;
using ElectroPiTask.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPiTask.Application.Services
{
    public class ProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            IRepository<Project> projectRepository,
            ILogger<ProjectService> logger)
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto)
        {
            _logger.LogInformation("Creating new project: {Name}", dto.Name);

            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            _logger.LogInformation("Project '{Name}' created successfully with ID {Id}", project.Name, project.Id);

            return MapToDto(project);
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            _logger.LogInformation("Retrieving all projects");

            var projects = await _projectRepository.GetAllAsync();
            return projects.Select(MapToDto).ToList();
        }

        public async Task<ProjectDto> GetProjectByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving project with ID {Id}", id);

            var project = await _projectRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Project), id);

            return MapToDto(project);
        }

        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto)
        {
            _logger.LogInformation("Updating project with ID {Id}", dto.Id);

            var project = await _projectRepository.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException(nameof(Project), dto.Id);

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChangesAsync();

            _logger.LogInformation("Project with ID {Id} updated successfully", dto.Id);

            return MapToDto(project);
        }

        public async Task DeleteProjectAsync(Guid id)
        {
            _logger.LogInformation("Deleting project with ID {Id}", id);

            var project = await _projectRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Project), id);

            await _projectRepository.DeleteAsync(project);
            await _projectRepository.SaveChangesAsync();

            _logger.LogInformation("Project with ID {Id} deleted successfully", id);
        }

        private static ProjectDto MapToDto(Project project) => new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
        };
    }
}
