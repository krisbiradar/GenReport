using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Infrastructure.Static.Constants;
using Microsoft.EntityFrameworkCore;

namespace GenReport.Endpoints.Onboarding
{
    /// <summary>
    /// Reset Password endpoint - validates OTP again, checks password match, and updates the password
    /// </summary>
    public class ResetPassword(ApplicationDbContext context) : Endpoint<ResetPasswordRequest, HttpResponse<Unit>>
    {
        private readonly ApplicationDbContext _context = context;

        public override void Configure()
        {
            Post("/reset-password");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
        {
            if (req.NewPassword != req.ConfirmPassword)
            {
                await SendAsync(new HttpResponse<Unit>(System.Net.HttpStatusCode.BadRequest, "Passwords do not match", ErrorMessages.PASSWORD_MISMATCH, ["NewPassword and ConfirmPassword must be identical"]), cancellation: ct);
                return;
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email, cancellationToken: ct);
            if (user == null)
            {
                await SendAsync(new HttpResponse<Unit>(System.Net.HttpStatusCode.BadRequest, "Invalid request", ErrorMessages.USER_NOT_FOUND, [$"No user found with email {req.Email}"]), cancellation: ct);
                return;
            }

            if (!user.VerifyOtp(req.Otp))
            {
                await SendAsync(new HttpResponse<Unit>(System.Net.HttpStatusCode.BadRequest, "Invalid or expired OTP", ErrorMessages.INVALID_OTP, ["OTP verification failed"]), cancellation: ct);
                return;
            }

            user.UpdatePassword(req.NewPassword);
            user.ClearOtp();
            await _context.SaveChangesAsync(ct);

            await SendAsync(new HttpResponse<Unit>(Unit.Value, "Password has been reset successfully."), cancellation: ct);
        }
    }
}
