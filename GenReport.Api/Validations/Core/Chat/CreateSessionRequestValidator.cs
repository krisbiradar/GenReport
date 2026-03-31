using FastEndpoints;
using FluentValidation;
using GenReport.Infrastructure.Models.HttpRequests.Core.Chat;

namespace GenReport.Api.Validations.Core.Chat
{
    public class CreateSessionRequestValidator : Validator<CreateSessionRequest>
    {
        public CreateSessionRequestValidator()
        {
            RuleFor(x => x.ModelId)
                .NotEmpty()
                .WithMessage("ModelId is required for creating a session.");

            RuleFor(x => x.ProviderId)
                .NotEmpty()
                .WithMessage("ProviderId is required for creating a session.");

            RuleFor(x => x.DatabaseConnectionId)
                .NotEmpty()
                .WithMessage("DatabaseConnectionId is required for creating a session.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required for creating a session.");
        }
    }
}
