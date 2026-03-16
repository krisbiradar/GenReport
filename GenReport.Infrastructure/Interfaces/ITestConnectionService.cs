using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;

namespace GenReport.Infrastructure.Interfaces
{
    public interface ITestConnectionService
    {
        public Task<(bool IsSuccess, string Message)> TestConnectionAsync(AddDatabaseRequest request, CancellationToken cancellationToken);
    }
}
