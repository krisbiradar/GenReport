using FastEndpoints;
using FluentValidation;
using GenReport.Infrastructure.Models.HttpRequests.Core.Ai.Configs;

namespace GenReport.Api.Validations.Core.Ai.Configs
{
    public class EditAiConfigRequestValidator : Validator<EditAiConfigRequest>
    {
        public EditAiConfigRequestValidator()
        {
            RuleFor(x => x)
                .Must(x => x.Value != null || x.IsActive.HasValue)
                .WithMessage("At least one property must be provided to update.");
        }
    }
}
