using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Infrastructure.Static.Constants;
using Microsoft.EntityFrameworkCore;

namespace GenReport.Endpoints.Onboarding
{
    /// <summary>
    /// Verify OTP endpoint - validates the OTP code for a given email
    /// </summary>
    public class VerifyOtp(ApplicationDbContext context) : Endpoint<VerifyOtpRequest, HttpResponse<Unit>>
    {
        private readonly ApplicationDbContext _context = context;

        public override void Configure()
        {
            Post("/verify-otp");
            AllowAnonymous();
        }

        public override async Task HandleAsync(VerifyOtpRequest req, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email, cancellationToken: ct);
            if (user == null)
            {
                await SendAsync(new HttpResponse<Unit>(System.Net.HttpStatusCode.BadRequest, "Invalid email or OTP", ErrorMessages.INVALID_OTP, [$"No user found with email {req.Email}"]), cancellation: ct);
                return;
            }

            if (user.OtpExpiry != null && DateTime.UtcNow > user.OtpExpiry)
            {
                await SendAsync(new HttpResponse<Unit>(System.Net.HttpStatusCode.BadRequest, "OTP has expired. Please request a new one.", ErrorMessages.OTP_EXPIRED, ["OTP expired"]), cancellation: ct);
                return;
            }

            if (!user.VerifyOtp(req.Otp))
            {
                await SendAsync(new HttpResponse<Unit>(System.Net.HttpStatusCode.BadRequest, "Invalid OTP code", ErrorMessages.INVALID_OTP, ["OTP does not match"]), cancellation: ct);
                return;
            }

            await SendAsync(new HttpResponse<Unit>(Unit.Value, "OTP verified successfully."), cancellation: ct);
        }
    }
}
