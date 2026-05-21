using ElectroPiTask.Application.DTOs.Project;
using ElectroPiTask.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPiTask.Application.DTOs.ProjectTask
{
    public class ProjectTaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public Domain.Enums.TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid ProjectId { get; set; }
    }
}
