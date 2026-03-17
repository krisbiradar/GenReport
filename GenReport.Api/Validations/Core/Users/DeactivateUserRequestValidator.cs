using FastEndpoints;
using FluentValidation;
using GenReport.Infrastructure.Models.HttpRequests.Core.Users;

namespace GenReport.Api.Validations.Core.Users
{
    public class DeactivateUserRequestValidator : Validator<DeactivateUserRequest>
    {
        public DeactivateUserRequestValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("User id is required.");
        }
    }
}
