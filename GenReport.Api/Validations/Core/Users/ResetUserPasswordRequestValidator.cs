using FastEndpoints;
using FluentValidation;
using GenReport.Infrastructure.Models.HttpRequests.Core.Users;

namespace GenReport.Api.Validations.Core.Users
{
    public class ResetUserPasswordRequestValidator : Validator<ResetUserPasswordRequest>
    {
        public ResetUserPasswordRequestValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User id is required.");
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).WithMessage("New password must be at least 8 characters.");
        }
    }
}
