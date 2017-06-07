using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleStatusPageDotNet.Models
{
    public interface IHealthCheckHelper
    {
        Task<IEnumerable<SiteHealthModel>> GetSiteHealthsAsync();
        Task<IEnumerable<ApiHealthModel>> GetApiHealthsAsync();
        Task<IEnumerable<HealthModel>> GetDbHealthsAsync();
    }
}