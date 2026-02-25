namespace GenReport.Validations.Onboarding
{
    using FastEndpoints;
    using FluentValidation;
    using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
    using GenReport.Infrastructure.Static.Externsions;

    public class ForgotPasswordRequestValidator : Validator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").Must(x => x.IsEmail()).WithMessage("Email address is not valid");
        }
    }
}
