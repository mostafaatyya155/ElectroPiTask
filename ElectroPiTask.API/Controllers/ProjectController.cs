using ElectroPiTask.Application.Common.Models;
using ElectroPiTask.Application.DTOs.Auth;
using ElectroPiTask.Application.DTOs.Project;
using ElectroPiTask.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPiTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(ProjectService projectService, ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> CreateProject(CreateProjectDto dto)
        {
            var result = await _projectService.CreateProjectAsync(dto);

            return Ok(ApiResponse<ProjectDto>.SuccessResponse(
                result,
                "Project created successfully."));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProjectDto>>>> GetAllProjects()
        {
            var result = await _projectService.GetAllProjectsAsync();

            return Ok(ApiResponse<List<ProjectDto>>.SuccessResponse(
                result,
                "All projects retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> GetProjectById(Guid id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);

            return Ok(ApiResponse<ProjectDto>.SuccessResponse(
                result,
                "Project retrieved successfully."));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> UpdateProjectById(UpdateProjectDto dto)
        {
            var result = await _projectService.UpdateProjectAsync(dto);

            return Ok(ApiResponse<ProjectDto>.SuccessResponse(
                result,
                "Project updated successfully."));

        }

        [HttpDelete("{projectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProject(Guid projectId)
        {
            await _projectService.DeleteProjectAsync(projectId);

            return Ok(ApiResponse<string>.SuccessResponse(
                "Project deleted successfully."));
        }
    }
}
