using FluentValidation;

namespace Platform.Application.Tenancy.CreateTenant;

public sealed class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Slug).NotEmpty().MaximumLength(100).Matches("^[a-z0-9-]+$")
            .WithMessage("Slug must be lowercase letters, digits, and hyphens only.");
        RuleFor(c => c.ConnectionStringSecretRef)
            .NotEmpty()
            .When(c => c.IsDedicated)
            .WithMessage("A dedicated tenant requires a connection string secret reference.");
    }
}
