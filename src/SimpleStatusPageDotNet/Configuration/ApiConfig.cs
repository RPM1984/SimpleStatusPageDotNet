namespace SimpleStatusPageDotNet.Configuration
{
    public class ApiConfig : SiteConfig
    {
        public string HealthEndpoint { get; set; }
        public int AvgResponseTimeThreshold { get; set; }
    }
}