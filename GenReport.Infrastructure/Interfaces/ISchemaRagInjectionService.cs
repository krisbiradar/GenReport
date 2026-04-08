using GenReport.Infrastructure.Models.AI;

namespace GenReport.Infrastructure.Interfaces
{
    /// <summary>
    /// Manages the structured injection of schema RAG data into the chat system message.
    /// The system message format is:
    /// <code>
    /// {base system prompt}
    /// &lt;&lt;&lt;GENREPORT_RAG_CONTEXT&gt;&gt;&gt;
    /// {serialised SchemaRagPayload JSON}
    /// </code>
    /// </summary>
    public interface ISchemaRagInjectionService
    {
        /// <summary>
        /// Build a complete system message from a static base prompt and a RAG payload.
        /// </summary>
        string BuildSystemMessage(string basePrompt, SchemaRagPayload payload);

        /// <summary>
        /// Extract the RAG payload from an existing system message.
        /// Returns <c>null</c> if the message contains no RAG block.
        /// </summary>
        SchemaRagPayload? ExtractPayload(string systemMessage);

        /// <summary>
        /// Extract the static base prompt portion from an existing system message
        /// (everything before the delimiter).
        /// </summary>
        string ExtractBasePrompt(string systemMessage);

        /// <summary>
        /// Replace just the RAG payload within an existing system message,
        /// preserving the base prompt.
        /// </summary>
        string UpdatePayload(string existingSystemMessage, SchemaRagPayload updatedPayload);

        /// <summary>
        /// Build a <see cref="SchemaRagPayload"/> from schema search results and database metadata.
        /// </summary>
        SchemaRagPayload BuildPayload(
            string databaseProvider,
            string databaseName,
            IReadOnlyList<SchemaSearchResult> searchResults);
    }
}
