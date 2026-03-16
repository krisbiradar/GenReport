using GenReport.DB.Domain.Enums;
using GenReport.Infrastructure.Interfaces;
using GenReport.Infrastructure.Models.HttpRequests.Core.Databases;
using System.Data.Common;

namespace GenReport.Infrastructure.SharedServices.Core.Databases
{
    public class TestConnectionService : ITestConnectionService
    {
        public async Task<(bool IsSuccess, string Message)> TestConnectionAsync(AddDatabaseRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string connectionString = GetConnectionString(request);
                var providerFactory = ResolveFactory(request.Provider);

                if (providerFactory == null)
                {
                    return (false, $"Provider {request.Provider} is not available in current runtime.");
                }

                await using var connection = providerFactory.CreateConnection();
                if (connection == null)
                {
                    return (false, $"Unable to create connection for provider {request.Provider}.");
                }

                connection.ConnectionString = connectionString;
                await connection.OpenAsync(cancellationToken);
                await connection.CloseAsync();

                return (true, "Database connection test successful.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private static string GetConnectionString(AddDatabaseRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                return request.ConnectionString;
            }

            return request.Provider switch
            {
                DbProvider.NpgSql => $"Host={request.HostName};Port={request.Port};Database={request.DatabaseName};Username={request.UserName};Password={request.Password};",
                DbProvider.SqlClient => $"Server={request.HostName},{request.Port};Database={request.DatabaseName};User Id={request.UserName};Password={request.Password};Encrypt=False;TrustServerCertificate=True;",
                DbProvider.MySqlConnector => $"Server={request.HostName};Port={request.Port};Database={request.DatabaseName};User Id={request.UserName};Password={request.Password};",
                DbProvider.Oracle => $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={request.HostName})(PORT={request.Port}))(CONNECT_DATA=(SERVICE_NAME={request.DatabaseName})));User Id={request.UserName};Password={request.Password};",
                DbProvider.MongoClient => string.Empty,
                _ => throw new NotSupportedException($"Provider {request.Provider} is not supported.")
            };
        }

        private static DbProviderFactory? ResolveFactory(DbProvider provider)
        {
            return provider switch
            {
                DbProvider.NpgSql => CreateFactory("Npgsql.NpgsqlFactory, Npgsql"),
                DbProvider.SqlClient => CreateFactory("Microsoft.Data.SqlClient.SqlClientFactory, Microsoft.Data.SqlClient")
                    ?? CreateFactory("System.Data.SqlClient.SqlClientFactory, System.Data.SqlClient"),
                DbProvider.MySqlConnector => CreateFactory("MySqlConnector.MySqlConnectorFactory, MySqlConnector"),
                DbProvider.Oracle => CreateFactory("Oracle.ManagedDataAccess.Client.OracleClientFactory, Oracle.ManagedDataAccess"),
               DbProvider.MongoClient => CreateFactory("MongoDB.Driver.MongoClientFactory, MongoDB.Driver.Core"),
                _ => null
            };
        }

        private static DbProviderFactory? CreateFactory(string typeName)
        {
            var factoryType = Type.GetType(typeName, throwOnError: false);
            if (factoryType == null)
            {
                return null;
            }

            var instanceProperty = factoryType.GetProperty("Instance");
            return instanceProperty?.GetValue(null) as DbProviderFactory;
        }
    }
}
