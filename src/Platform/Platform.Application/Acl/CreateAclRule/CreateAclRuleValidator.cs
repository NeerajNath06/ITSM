using System.Text.Json;
using FluentValidation;

namespace Platform.Application.Acl.CreateAclRule;

public sealed class CreateAclRuleValidator : AbstractValidator<CreateAclRuleCommand>
{
    public CreateAclRuleValidator()
    {
        RuleFor(c => c.TenantId).NotEmpty();
        RuleFor(c => c.TableName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.RequiredRoleId).NotEmpty();
        RuleFor(c => c.ConditionExpression)
            .Must(BeValidJson)
            .When(c => c.ConditionExpression is not null)
            .WithMessage("ConditionExpression must be valid JSON.");
    }

    private static bool BeValidJson(string? json)
    {
        try
        {
            using var _ = JsonDocument.Parse(json!);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
