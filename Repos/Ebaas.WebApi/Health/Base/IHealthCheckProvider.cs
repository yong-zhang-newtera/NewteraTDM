using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ebaas.WebApi.Health
{
    /// <summary>
    /// Defines the interface for a health check provider.
    /// </summary>
    public interface IHealthCheckProvider
    {
        /// <summary>
        /// Returns the health heck info.
        /// </summary>
        Task<List<HealthCheckItemResult>> GetHealthCheckAsync();

        /// <summary>
        /// Defines the order of this provider in the results.
        /// </summary>
        int SortOrder { get; }
    }
}