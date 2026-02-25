namespace GenReport.Validations.Onboarding
{
    using FastEndpoints;
    using FluentValidation;
    using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
    using GenReport.Infrastructure.Static.Externsions;

    public class VerifyOtpRequestValidator : Validator<VerifyOtpRequest>
    {
        public VerifyOtpRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").Must(x => x.IsEmail()).WithMessage("Email address is not valid");
            RuleFor(x => x.Otp).NotEmpty().WithMessage("OTP is required").Length(6).WithMessage("OTP must be exactly 6 digits");
        }
    }
}
