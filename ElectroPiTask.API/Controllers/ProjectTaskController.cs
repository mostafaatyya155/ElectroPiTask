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
    [Authorize]
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
            var result = await _projectTaskService.CreateProjectTaskAsync(dto);

            return Ok(ApiResponse<ProjectTaskDto>.SuccessResponse(
                result,
                "Task Created successfully."));
        }

        [HttpGet("{projectId}")]
        public async Task<ActionResult<ApiResponse<List<ProjectTaskDto>>>> GetAllProjects(Guid projectId)
        {
            var result = await _projectTaskService.GetTasksByProjectAsync(projectId);

            return Ok(ApiResponse<List<ProjectTaskDto>>.SuccessResponse(
                result,
                "All tasks for the project retrieved successfully."));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ProjectTaskDto>>> UpdateProjectById(UpdateProjectTaskStatusDto dto)
        {
            var result = await _projectTaskService.UpdateProjectTaskStatusAsync(dto);

            return Ok(ApiResponse<ProjectTaskDto>.SuccessResponse(
                result,
                "Task status updated successfully."));
        }

        [HttpDelete("{taskId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProject(Guid taskId)
        {
            await _projectTaskService.DeleteTaskAsync(taskId);

            return Ok(ApiResponse<string>.SuccessResponse(
                "Task deleted successfully."));
        }
    }
}
