namespace SimpleStatusPageDotNet.Models
{
    public class ApiHealthModel : SiteHealthModel
    {
        public int AvgResponseTime { get; set; }
    }
}