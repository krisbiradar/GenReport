using FastEndpoints;
using GenReport.Domain.DBContext;
using GenReport.Infrastructure.Models.HttpRequests.Onboarding;
using GenReport.Infrastructure.Models.Shared;
using GenReport.Infrastructure.Static.Constants;
using Microsoft.EntityFrameworkCore;

namespace GenReport.Endpoints.Onboarding
{
    /// <summary>
    /// Forgot Password endpoint - generates an OTP and returns it (for dev/testing)
    /// </summary>
    public class ForgotPassword(ApplicationDbContext context) : Endpoint<ForgotPasswordRequest, HttpResponse<Unit>>
    {
        private readonly ApplicationDbContext _context = context;

        public override void Configure()
        {
            Post("/forgot-password");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ForgotPasswordRequest req, CancellationToken ct)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == req.Email, cancellationToken: ct);
            if (user == null)
            {
                // Return success even if user not found to prevent email enumeration
                await SendAsync(new HttpResponse<Unit>(Unit.Value, "If the email exists, a verification code has been sent."), cancellation: ct);
                return;
            }

            var otp = user.SetOtp();
            await _context.SaveChangesAsync(ct);

            // In production, send OTP via email (FluentEmail). For now, include it in response for testing.
            await SendAsync(new HttpResponse<Unit>(Unit.Value, $"Verification code sent. (Dev OTP: {otp})"), cancellation: ct);
        }
    }
}
