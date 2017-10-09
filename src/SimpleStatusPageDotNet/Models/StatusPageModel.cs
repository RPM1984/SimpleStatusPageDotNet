using System.Collections.Generic;
using System.Linq;

namespace SimpleStatusPageDotNet.Models
{
    public class StatusPageModel
    {
        public string AppName { get; set; }
        public string AppLogoUrl { get; set; }

        public bool IsOkOverall => Apis.All(site => !site.IsUnhealthy()) &&
                                   Sites.All(site => !site.IsUnhealthy()) &&
                                   KPIs.All(site => !site.IsUnhealthy()) &&
                                   ThirdPartyServices.All(site => !site.IsUnhealthy()) &&
                                   DBs.All(db => !db.IsUnhealthy());

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