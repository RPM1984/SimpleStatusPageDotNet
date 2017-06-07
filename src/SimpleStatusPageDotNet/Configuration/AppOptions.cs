using System.Collections.Generic;

namespace SimpleStatusPageDotNet.Configuration
{
    public class AppOptions
    {
        public string AppName { get; set; }
        public string AppLogoUrl { get; set; }
        public IEnumerable<SiteConfig> Sites { get; set; }
        public IEnumerable<ApiConfig> Apis { get; set; }
        public IEnumerable<DbConfig> Dbs { get; set; }
    }
}