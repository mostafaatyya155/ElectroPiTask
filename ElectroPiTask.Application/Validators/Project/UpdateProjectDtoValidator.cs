using ElectroPiTask.Application.DTOs.Project;
using FluentValidation;

namespace ElectroPiTask.Application.Validators.Project
{
    public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
    {
        public UpdateProjectDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Project ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(200).WithMessage("Project name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");
        }
    }
}
