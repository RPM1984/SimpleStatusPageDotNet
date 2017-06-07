using System.Collections.Generic;
using System.Linq;

namespace SimpleStatusPageDotNet.Models
{
    public class StatusPageModel
    {
        public string AppName { get; set; }
        public string AppLogoUrl { get; set; }

        public bool IsOkOverall => Apis.All(site => site.IsHealthy()) &&
                                   Sites.All(site => site.IsHealthy()) &&
                                   KPIs.All(site => site.IsHealthy()) &&
                                   ThirdPartyServices.All(site => site.IsHealthy()) &&
                                   DBs.All(db => db.IsHealthy());

        public string OverallStatus => IsOkOverall
                                           ? "All systems operational!"
                                           : "One or more systems are non-operational!";

        public string OverallStatusCssClass => IsOkOverall
                                                   ? "healthy"
                                                   : "unhealthy";

        public IEnumerable<SiteHealthModel> Sites { get; set; }
        public IEnumerable<ApiHealthModel> Apis { get; set; }
        public IEnumerable<KPIModel> KPIs { get; set; }
        public IEnumerable<HealthModel> ThirdPartyServices { get; set; }
        public IEnumerable<HealthModel> DBs { get; set; }
    }
}