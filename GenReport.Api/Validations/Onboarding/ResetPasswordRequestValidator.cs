namespace GenReport.Validations.Onboarding
{
    using FastEndpoints;
    using FluentValidation;
    using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
    using GenReport.Infrastructure.Static.Externsions;

    public class ResetPasswordRequestValidator : Validator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").Must(x => x.IsEmail()).WithMessage("Email address is not valid");
            RuleFor(x => x.Otp).NotEmpty().WithMessage("OTP is required").Length(6).WithMessage("OTP must be exactly 6 digits");
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required").MinimumLength(8).WithMessage("Password must be at least 8 characters");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required").Equal(x => x.NewPassword).WithMessage("Passwords do not match");
        }
    }
}
