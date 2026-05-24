using ElectroPiTask.Application.DTOs.Auth;
using ElectroPiTask.Domain.Enums;
using FluentValidation;

namespace ElectroPiTask.Application.Validators.Auth
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(255).WithMessage("First name must not exceed 255 characters.")
                .Matches(@"^[\p{L}\s\-']+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(255).WithMessage("Last name must not exceed 255 characters.")
                .Matches(@"^[\p{L}\s\-']+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage($"Role must be a valid value: {string.Join(", ", Enum.GetNames<UserRole>())}.");
        }
    }
}
