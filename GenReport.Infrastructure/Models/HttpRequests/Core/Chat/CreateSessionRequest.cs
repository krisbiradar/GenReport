namespace GenReport.Infrastructure.Models.HttpRequests.Core.Chat
{
    public class CreateSessionRequest
    {
        public string? ModelId { get; set; }
        public string? ProviderId { get; set; }
        public string? DatabaseConnectionId { get; set; }
        public string? Title { get; set; }
    }
}
