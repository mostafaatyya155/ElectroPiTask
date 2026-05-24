using ElectroPiTask.Application.DTOs.ProjectTask;
using ElectroPiTask.Domain.Enums;
using FluentValidation;

namespace ElectroPiTask.Application.Validators.ProjectTask
{
    public class CreateProjectTaskDtoValidator : AbstractValidator<CreateProjectTaskDto>
    {
        public CreateProjectTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(255).WithMessage("Task title must not exceed 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage($"Status must be a valid value: {string.Join(", ", Enum.GetNames<Domain.Enums.TaskStatus>())}.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage($"Priority must be a valid value: {string.Join(", ", Enum.GetNames<TaskPriority>())}.");

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("ProjectId is required.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
                .When(x => x.DueDate.HasValue);
        }
    }
}
