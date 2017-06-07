namespace SimpleStatusPageDotNet.Models
{
    public class HealthModel
    {
        public string Name { get; set; }
        public HealthStatus Status { get; set; }
        public string CssClass => Status.ToString().ToLowerInvariant();
    }

    public static class HealthModelExtensions
    {
        public static bool IsHealthy(this HealthModel healthModel)
        {
            return healthModel.Status == HealthStatus.Healthy;
        }
    }
}