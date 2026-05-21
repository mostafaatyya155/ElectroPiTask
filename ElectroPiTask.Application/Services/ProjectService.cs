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
            _logger.LogInformation("Creating new project");

            //Create project
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            _logger.LogInformation("Project {project.Name} created successfully", project.Name);

            //Map to DTO
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
            };

        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            _logger.LogInformation("Retreiving all projects");

            var projects = await _projectRepository.GetAllAsync();
            
            List<ProjectDto> result = new List<ProjectDto>();
            foreach (var project in projects)
            {
                result.Add(new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                });
            }
            return result;
        }


        public async Task<ProjectDto> GetProjectByIdAsync(Guid id)
        {
            _logger.LogInformation("Retreiving project with ID {id}",id);

            var project = await _projectRepository.GetByIdAsync(id);

            if(project == null)
            {
                _logger.LogWarning("Project not found with ID {id}", id);
                throw new Exception("Project not found");
            }
            else
            {
                return new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description
                };
            }
        }

        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto)
        {
            _logger.LogInformation("Updating project with ID {dto.Id}", dto.Id);

            var project = await _projectRepository.GetByIdAsync(dto.Id);

            if (project == null)
            {
                _logger.LogWarning("Project not found with ID {Id}", dto.Id);
                throw new Exception("Project not found");
            }
            else
            {
                project.Name = dto.Name;
                project.Description = dto.Description;
                await _projectRepository.UpdateAsync(project);
                await _projectRepository.SaveChangesAsync();

                return new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description
                };
            }
        }

        public async Task DeleteProjectAsync(Guid id)
        {
            _logger.LogInformation("Deleting Project with ID {id}", id);

            var projectToDelete = await _projectRepository.GetByIdAsync(id);

            if (projectToDelete != null)
            {
                await _projectRepository.DeleteAsync(projectToDelete);
                await _projectRepository.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Project not found with ID {id}", id);
                throw new Exception("Project not found");
            }
        }
    }
}
