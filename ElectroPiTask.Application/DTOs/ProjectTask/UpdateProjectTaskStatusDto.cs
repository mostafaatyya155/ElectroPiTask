using ElectroPiTask.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPiTask.Application.DTOs.ProjectTask
{
    public class UpdateProjectTaskStatusDto
    {
        public Guid Id { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; }
    }
}
