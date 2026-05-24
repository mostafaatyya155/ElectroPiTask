using ElectroPiTask.Application.Validators.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPiTask.API.Extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            // Register all validators in the Application assembly automatically
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

            // Wire FluentValidation into the MVC pipeline
            services.AddFluentValidationAutoValidation();

            // Override the default 400 response to use the project's ApiResponse<T> shape
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors.Select(er =>
                            string.IsNullOrWhiteSpace(er.ErrorMessage)
                                ? er.Exception?.Message ?? "Unknown validation error."
                                : er.ErrorMessage))
                        .ToList();

                    var response = new
                    {
                        success = false,
                        message = "Validation failed.",
                        data = (object?)null,
                        errors
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
