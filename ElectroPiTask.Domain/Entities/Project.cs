using ElectroPiTask.Domain.Common;

namespace ElectroPiTask.Domain.Entities
{
    public sealed class Project : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<ProjectTask> Tasks { get; set; } = [];

    }
}
