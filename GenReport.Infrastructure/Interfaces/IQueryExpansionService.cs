namespace GenReport.Infrastructure.Interfaces
{
    /// <summary>
    /// Generates semantically varied re-phrasings of a user query to improve vector-search recall.
    /// The original query is always included in the returned list as the first element.
    /// </summary>
    public interface IQueryExpansionService
    {
        /// <summary>
        /// Expands <paramref name="originalQuery"/> into several alternative queries using an LLM.
        /// Falls back gracefully (returns only the original) if the LLM call fails or is unavailable.
        /// </summary>
        /// <param name="originalQuery">The raw user question / combined conversation query.</param>
        /// <param name="provider">AI provider name (e.g. "openai", "anthropic").</param>
        /// <param name="apiKey">Decrypted API key for the session's AI connection.</param>
        /// <param name="model">Model identifier to use for expansion.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>
        /// An ordered list starting with <paramref name="originalQuery"/>, followed by LLM-generated variants.
        /// </returns>
        Task<IReadOnlyList<string>> ExpandAsync(
            string originalQuery,
            string provider,
            string apiKey,
            string model,
            CancellationToken ct = default);
    }
}
