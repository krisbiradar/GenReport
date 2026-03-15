namespace GenReport.Infrastructure.Models.HttpResponse.Core.Databases
{
    public class DatabaseResponse
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string ServerAddress { get; set; }
        public int Port { get; set; }
        public required string Username { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
