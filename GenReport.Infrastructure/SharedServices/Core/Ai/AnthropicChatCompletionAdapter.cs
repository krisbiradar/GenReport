using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GenReport.Infrastructure.SharedServices.Core.Ai
{
    /// <summary>
    /// Adapts the Anthropic.SDK <see cref="AnthropicClient"/> to Semantic Kernel's
    /// <see cref="IChatCompletionService"/> interface so it can be used interchangeably
    /// with OpenAI/Gemini connectors via the factory.
    /// </summary>
    public sealed class AnthropicChatCompletionAdapter : IChatCompletionService
    {
        private readonly AnthropicClient _client;
        private readonly string _model;

        public IReadOnlyDictionary<string, object?> Attributes { get; }

        public AnthropicChatCompletionAdapter(string apiKey, string model)
        {
            _client = new AnthropicClient(apiKey);
            _model = model;
            Attributes = new Dictionary<string, object?>
            {
                ["ModelId"] = model,
                ["Provider"] = "Anthropic"
            };
        }

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            var messages = chatHistory
                .Where(m => m.Role != AuthorRole.System)
                .Select(m => new Message
                {
                    Role = m.Role == AuthorRole.User ? RoleType.User : RoleType.Assistant,
                    Content = [new Anthropic.SDK.Messaging.TextContent { Text = m.Content ?? string.Empty }]
                })
                .ToList();

            var systemPrompt = chatHistory
                .Where(m => m.Role == AuthorRole.System)
                .Select(m => m.Content)
                .FirstOrDefault();

            var parameters = new MessageParameters
            {
                Model = _model,
                Messages = messages,
                MaxTokens = 1024,
                System = string.IsNullOrEmpty(systemPrompt) ? null : [new SystemMessage(systemPrompt)]
            };

            var response = await _client.Messages.GetClaudeMessageAsync(parameters, cancellationToken);

            var content = response.Message?.ToString() ?? string.Empty;

            return [new ChatMessageContent(AuthorRole.Assistant, content)];
        }

        public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // For test connection purposes, streaming is not needed.
            // Delegate to the non-streaming version.
            var results = await GetChatMessageContentsAsync(chatHistory, executionSettings, kernel, cancellationToken);
            foreach (var result in results)
            {
                yield return new StreamingChatMessageContent(result.Role, result.Content);
            }
        }
    }
}
