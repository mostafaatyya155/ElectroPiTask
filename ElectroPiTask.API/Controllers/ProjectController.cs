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
            try
            {

                var result = await _projectService.CreateProjectAsync(dto);

                return Ok(ApiResponse<ProjectDto>.SuccessResponse(
                    result,
                    "Project created successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new project");
                return BadRequest(ApiResponse<ProjectDto>.FailureResponse(ex.Message));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ProjectDto>>>> GetAllProjects()
        {
            try
            {

                var result = await _projectService.GetAllProjectsAsync();

                return Ok(ApiResponse<List<ProjectDto>>.SuccessResponse(
                    result,
                    "All projects retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all projects");
                return BadRequest(ApiResponse<ProjectDto>.FailureResponse(ex.Message));
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> GetProjectById(Guid id)
        {
            try
            {

                var result = await _projectService.GetProjectByIdAsync(id);

                return Ok(ApiResponse<ProjectDto>.SuccessResponse(
                    result,
                    "Project retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project");
                return BadRequest(ApiResponse<ProjectDto>.FailureResponse(ex.Message));
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> UpdateProjectById(UpdateProjectDto dto)
        {
            try
            {
                var result = await _projectService.UpdateProjectAsync(dto);

                return Ok(ApiResponse<ProjectDto>.SuccessResponse(
                    result,
                    "Project updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project");
                return BadRequest(ApiResponse<ProjectDto>.FailureResponse(ex.Message));
            }
        }

        [HttpDelete("{projectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProject(Guid projectId)
        {
            try
            {
                await _projectService.DeleteProjectAsync(projectId);

                return Ok(ApiResponse<string>.SuccessResponse(
                    "Project deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project");
                return BadRequest(ApiResponse<ProjectDto>.FailureResponse(ex.Message));
            }
        }
    }
}
