using FluentValidation;

namespace Platform.Application.Identity.InviteUser;

public sealed class InviteUserValidator : AbstractValidator<InviteUserCommand>
{
    public InviteUserValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.Username).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(c => c.DisplayName).NotEmpty().MaximumLength(200);
    }
}
