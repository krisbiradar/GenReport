namespace GenReport.Infrastructure.Models.HttpRequests.Core.Chat
{
    public class UpdateSessionProviderRequest
    {
        /// <summary>The ID of the AI connection to assign to this session.</summary>
        public required long AiConnectionId { get; set; }
    }
}
