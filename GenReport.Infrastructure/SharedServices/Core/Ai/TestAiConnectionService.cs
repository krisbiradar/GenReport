using GenReport.Infrastructure.Interfaces;
using GenReport.Infrastructure.Models.HttpRequests.Core.Ai;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GenReport.Infrastructure.SharedServices.Core.Ai
{
    /// <summary>
    /// Tests an AI connection by sending a simple prompt via IChatCompletionService
    /// and checking for a valid response.
    /// </summary>
    public class TestAiConnectionService(
        IChatCompletionFactory chatCompletionFactory,
        ILogger<TestAiConnectionService> logger) : ITestAiConnectionService
    {
        private const string TestPrompt = "Hi, respond with OK if you can read this.";

        public async Task<(bool IsSuccess, string Message)> TestConnectionAsync(TestAiConnectionRequest request, CancellationToken ct)
        {
            try
            {
                // Build the IChatCompletionService via factory
                var chatService = chatCompletionFactory.Create(
                    request.Provider,
                    request.ApiKey,
                    request.DefaultModel,
                    request.ChatEndpointUrl);

                // Send a test prompt
                var chatHistory = new ChatHistory();
                chatHistory.AddUserMessage(TestPrompt);

                var response = await chatService.GetChatMessageContentAsync(chatHistory, cancellationToken: ct);

                var text = response?.Content;

                if (string.IsNullOrWhiteSpace(text))
                    return (false, "AI responded but returned an empty message.");

                logger.LogInformation(
                    "AI connection test succeeded for {Provider} ({Model}). Response: {Response}",
                    request.Provider, request.DefaultModel, text);

                return (true, $"Connection successful. AI responded: \"{text}\"");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AI connection test failed for provider {Provider}", request.Provider);
                return (false, $"Connection test failed: {ex.Message}");
            }
        }
    }
}
