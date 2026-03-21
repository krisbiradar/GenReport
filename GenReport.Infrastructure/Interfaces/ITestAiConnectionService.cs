using GenReport.Infrastructure.Models.HttpRequests.Core.Ai;

namespace GenReport.Infrastructure.Interfaces
{
    /// <summary>
    /// Tests an AI connection by sending a simple prompt and verifying a valid response.
    /// </summary>
    public interface ITestAiConnectionService
    {
        /// <summary>
        /// Sends a lightweight test prompt to the AI provider using the
        /// supplied connection details and returns the result.
        /// </summary>
        Task<(bool IsSuccess, string Message)> TestConnectionAsync(TestAiConnectionRequest request, CancellationToken ct);
    }
}
