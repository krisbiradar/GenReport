namespace GenReport.Infrastructure.Models.HttpResponse.Core.Databases
{
    public class DatabaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public required string Name { get; set; }
        public required string DatabaseAlias { get; set; }
        public required string DatabaseType { get; set; }
        public required string HostName { get; set; }
        public int Port { get; set; }
        public required string UserName { get; set; }
        public required string DatabaseName { get; set; }
        // Password might not usually be returned for security, but we keep it in payload as per comment.
        public string? Password { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
