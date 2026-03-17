using GenReport.Infrastructure.Interfaces;
using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace GenReport.Infrastructure.SharedServices.Core.Databases
{
    public class TestConnectionService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : ITestConnectionService
    {
        private const string GoServiceTestConnectionPath = "/go/connections/test";

        public async Task<(bool IsSuccess, string Message)> TestConnectionAsync(AddDatabaseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var currentRequest = httpContextAccessor.HttpContext?.Request;
                if (currentRequest == null)
                {
                    return (false, "Unable to resolve current request context for Go service proxy.");
                }

                var goServiceUri = new Uri($"{currentRequest.Scheme}://{currentRequest.Host}{GoServiceTestConnectionPath}");
                var client = httpClientFactory.CreateClient();
                using var response = await client.PostAsJsonAsync(goServiceUri, request, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrWhiteSpace(responseBody))
                    {
                        return (false, responseBody);
                    }

                    return (false, $"Go service returned {(int)response.StatusCode} {response.StatusCode}.");
                }

                return (true, string.IsNullOrWhiteSpace(responseBody) ? "Database connection test successful." : responseBody);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
