using ElectroPiTask.Application.Common.Models;
using ElectroPiTask.Application.DTOs.Project;
using ElectroPiTask.Application.DTOs.ProjectTask;
using ElectroPiTask.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPiTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTaskController : ControllerBase
    {
        private readonly ProjectTaskService _projectTaskService;
        private readonly ILogger<ProjectTaskController> _logger;

        public ProjectTaskController(ProjectTaskService projectService, ILogger<ProjectTaskController> logger)
        {
            _projectTaskService = projectService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProjectTaskDto>>> CreateProjectTask(CreateProjectTaskDto dto)
        {
            try
            {

                var result = await _projectTaskService.CreateProjectTaskAsync(dto);

                return Ok(ApiResponse<ProjectTaskDto>.SuccessResponse(
                    result,
                    "User registered successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new project");
                return BadRequest(ApiResponse<ProjectTaskDto>.FailureResponse(ex.Message));
            }
        }

        [HttpGet("{projectId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<ProjectTaskDto>>>> GetAllProjects(Guid projectId)
        {
            try
            {

                var result = await _projectTaskService.GetTasksByProjectAsync(projectId);

                return Ok(ApiResponse<List<ProjectTaskDto>>.SuccessResponse(
                    result,
                    "All tasks for the project retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all task for the project");
                return BadRequest(ApiResponse<List<ProjectTaskDto>>.FailureResponse(ex.Message));
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProjectTaskDto>>> UpdateProjectById(UpdateProjectTaskStatusDto dto)
        {
            try
            {
                var result = await _projectTaskService.UpdateProjectTaskStatusAsync(dto);

                return Ok(ApiResponse<ProjectTaskDto>.SuccessResponse(
                    result,
                    "Task status updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status");
                return BadRequest(ApiResponse<ProjectTaskDto>.FailureResponse(ex.Message));
            }
        }

        [HttpDelete("{taskId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProject(Guid taskId)
        {
            try
            {
                await _projectTaskService.DeleteTaskAsync(taskId);

                return Ok(ApiResponse<string>.SuccessResponse(
                    "Task deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return BadRequest(ApiResponse<ProjectDto>.FailureResponse(ex.Message));
            }
        }
    }
}
