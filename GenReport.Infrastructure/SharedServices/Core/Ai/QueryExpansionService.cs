using System.Text.Json;
using GenReport.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GenReport.Infrastructure.SharedServices.Core.Ai
{
    /// <summary>
    /// Uses the session's LLM to produce semantically varied re-phrasings of the user query,
    /// broadening recall in the subsequent vector similarity search over database schema objects.
    /// <para>
    /// The original query is always element 0 in the returned list.
    /// If the LLM call fails, a single silent retry is attempted; after that the service
    /// falls back to returning only the original query with no error propagated.
    /// </para>
    /// </summary>
    public sealed class QueryExpansionService(
        IChatCompletionFactory chatCompletionFactory,
        ILogger<QueryExpansionService> logger) : IQueryExpansionService
    {
        private const int VariantCount = 3;
        private const int MaxRetries = 1;

        // Compact system prompt — asks for a JSON array of short alternative queries.
        private static readonly string ExpansionSystemPrompt =
            $"""
            You are a query rewriting assistant for a SQL database schema search system.
            Given a user question, produce exactly {VariantCount} alternative phrasings that would help
            retrieve relevant database tables, columns, views, or stored procedures via semantic search.
            Each alternative should approach the same information need from a different angle
            (e.g. synonyms, more specific terminology, abbreviated forms).
            
            Respond with ONLY a JSON array of {VariantCount} strings, no explanation, no markdown.
            Example: ["variant one", "variant two", "variant three"]
            """;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <inheritdoc />
        public async Task<IReadOnlyList<string>> ExpandAsync(
            string originalQuery,
            string provider,
            string apiKey,
            string model,
            CancellationToken ct = default)
        {
            var variants = await TryExpandWithRetryAsync(originalQuery, provider, apiKey, model, ct);

            var all = new List<string>(variants.Count + 1) { originalQuery };
            all.AddRange(variants.Where(v => !string.Equals(v, originalQuery, StringComparison.OrdinalIgnoreCase)));

            logger.LogInformation(
                "Query expansion produced {Count} variant(s) (original + {Extra}) for RAG search.",
                all.Count, variants.Count);

            return all;
        }

        // ── Private helpers ───────────────────────────────────────────────────────────

        private async Task<List<string>> TryExpandWithRetryAsync(
            string originalQuery,
            string provider,
            string apiKey,
            string model,
            CancellationToken ct)
        {
            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    var variants = await CallLlmForVariantsAsync(originalQuery, provider, apiKey, model, ct);
                    if (variants.Count > 0)
                        return variants;

                    logger.LogWarning(
                        "Query expansion attempt {Attempt} returned zero variants. {Retry}",
                        attempt + 1,
                        attempt < MaxRetries ? "Retrying..." : "Falling back to original query only.");
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogWarning(ex,
                        "Query expansion attempt {Attempt} failed. {Retry}",
                        attempt + 1,
                        attempt < MaxRetries ? "Retrying..." : "Falling back to original query only.");
                }
            }

            return [];
        }

        private async Task<List<string>> CallLlmForVariantsAsync(
            string originalQuery,
            string provider,
            string apiKey,
            string model,
            CancellationToken ct)
        {
            var chatService = chatCompletionFactory.Create(provider, apiKey, model);

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(ExpansionSystemPrompt);
            chatHistory.AddUserMessage(originalQuery);

            var response = await chatService.GetChatMessageContentAsync(
                chatHistory, cancellationToken: ct);

            var responseText = response?.Content?.Trim();
            if (string.IsNullOrWhiteSpace(responseText))
                return [];

            return ParseVariants(responseText);
        }

        private List<string> ParseVariants(string responseText)
        {
            // Strip markdown code fences if any
            var cleaned = responseText;
            if (cleaned.StartsWith("```"))
            {
                var newline = cleaned.IndexOf('\n');
                if (newline >= 0)
                    cleaned = cleaned[(newline + 1)..];
                if (cleaned.EndsWith("```"))
                    cleaned = cleaned[..^3];
                cleaned = cleaned.Trim();
            }

            try
            {
                var parsed = JsonSerializer.Deserialize<List<string>>(cleaned, JsonOptions);
                if (parsed is { Count: > 0 })
                {
                    // De-duplicate and filter empties; keep up to VariantCount
                    return parsed
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Take(VariantCount)
                        .ToList();
                }
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex,
                    "Failed to parse query expansion response as JSON array: {Response}",
                    cleaned.Length > 200 ? cleaned[..200] + "..." : cleaned);
            }

            return [];
        }
    }
}
