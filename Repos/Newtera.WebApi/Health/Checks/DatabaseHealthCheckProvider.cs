using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Newtera.Server.DB;

namespace Newtera.WebApi.Health.Checks
{
    /// <summary>
    /// Provides health info about the main database.
    /// </summary>
    public class DatabaseHealthCheckProvider : IHealthCheckProvider
    {
        /// <summary>
        /// Tries to connect to the application's main database.
        /// </summary>
        public Task<List<HealthCheckItemResult>> GetHealthCheckAsync()
        {
            DatabaseConfig dbConfig = DatabaseConfig.Instance;
            var results = new List<HealthCheckItemResult>();
            var result = new HealthCheckItemResult("Relational Database", SortOrder, "Checks the database", "Checks whether the database can be accessed.", dbConfig.GetDatabaseType().ToString());
            results.Add(result);

            IDbConnection con = null;
            try
            {
                var dataProvider = DataProviderFactory.Instance.Create();
                con = dataProvider.Connection;
                result.HealthState = HealthState.Healthy;
                result.Messages.Add($"Successfully connecting to database at {GetDatabaseServer(dbConfig.GetConnectionString())}.");
            }
            catch (Exception ex)
            {
                result.HealthState = HealthState.Unhealthy;
                result.Messages.Add($"Error connecting to database at {GetDatabaseServer(dbConfig.GetConnectionString())} due to {ex.Message} with detail {ex.StackTrace}.");
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }

            return Task.FromResult(results);
        }

        /// <summary>
        /// Defines the order of this provider in the results.
        /// </summary>
        public int SortOrder { get; } = 20;

        private string GetDatabaseServer(string connectionString)
        {
            string[] keyValuePairs = connectionString?.Split(';') ?? new string[0];
            foreach (string keyValuePair in keyValuePairs)
            {
                string[] keyAndValue = keyValuePair.Split('=');
                if (keyAndValue.Length == 2 && keyAndValue[0].ToUpper() == "SERVER")
                {
                    return keyAndValue[1];
                }
            }

            return "Unknown";
        }
    }
}