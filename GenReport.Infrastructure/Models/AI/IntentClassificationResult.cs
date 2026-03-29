using System.Text.Json.Serialization;

namespace GenReport.Infrastructure.Models.AI
{
    /// <summary>
    /// Supported chat intents for the intent classifier.
    /// </summary>
    public enum ChatIntent
    {
        Greeting,
        BotInfo,
        DatabaseQuery,
        ReportGenerate,
        Sensitive,
        OutOfScope
    }

    /// <summary>
    /// Result returned by the intent classifier LLM call.
    /// Deserialized from the controlled JSON output.
    /// </summary>
    public class IntentClassificationResult
    {
        [JsonPropertyName("intent")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChatIntent Intent { get; set; } = ChatIntent.OutOfScope;

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }
}
