    using ElectroPiTask.Application.DTOs.ProjectTask;
using FluentValidation;

namespace ElectroPiTask.Application.Validators.ProjectTask
{
    public class UpdateProjectTaskStatusDtoValidator : AbstractValidator<UpdateProjectTaskStatusDto>
    {
        public UpdateProjectTaskStatusDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Task ID is required.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage($"Status must be a valid value: {string.Join(", ", Enum.GetNames<Domain.Enums.TaskStatus>())}.");
        }
    }
}
