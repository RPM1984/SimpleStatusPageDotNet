using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shouldly;
using SimpleStatusPageDotNet.Configuration;

namespace SimpleStatusPageDotNet.Models
{
    public class HealthCheckHelper : IHealthCheckHelper
    {
        private const string UserAgentHeaderName = "User-Agent";

        private const string UserAgentHeaderValue =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

        private readonly Dictionary<SiteConfig, HttpClient> _sites;
        private readonly Dictionary<ApiConfig, HttpClient> _apis;
        private readonly IEnumerable<DbConfig> _dbs;

        public HealthCheckHelper(IOptions<AppOptions> appOptions)
        {
            appOptions.ShouldNotBeNull();

            _sites = appOptions.Value.Sites != null &&
                     appOptions.Value.Sites.Any()
                         ? appOptions.Value.Sites.ToDictionary(site => site,
                                                               site => new HttpClient
                                                               {
                                                                   BaseAddress = site.BaseAddress,
                                                                   Timeout = TimeSpan.FromSeconds(5),
                                                                   DefaultRequestHeaders =
                                                                   {
                                                                       {UserAgentHeaderName, UserAgentHeaderValue}
                                                                   }
                                                               })
                         : new Dictionary<SiteConfig, HttpClient>();
            _apis = appOptions.Value.Apis != null &&
                    appOptions.Value.Apis.Any()
                        ? appOptions.Value.Apis.ToDictionary(api => api,
                                                             api => new HttpClient
                                                             {
                                                                 BaseAddress = api.BaseAddress,
                                                                 Timeout = TimeSpan.FromSeconds(5)
                                                             })
                        : new Dictionary<ApiConfig, HttpClient>();

            _dbs = appOptions.Value.Dbs;
        }

        public async Task<IEnumerable<SiteHealthModel>> GetSiteHealthsAsync()
        {
            return await Task.WhenAll(_sites.Select(GetSiteHealthAsync))
                             .ConfigureAwait(false);
        }

        private static async Task<SiteHealthModel> GetSiteHealthAsync(KeyValuePair<SiteConfig, HttpClient> site)
        {
            site.ShouldNotBeNull();

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Head, ""))
                {
                    using (var response = await site.Value.SendAsync(request)
                                                    .ConfigureAwait(false))
                    {
                        return new ApiHealthModel
                        {
                            Name = site.Key.Name,
                            Url = site.Key.BaseAddress.AbsoluteUri,
                            Status = response.IsSuccessStatusCode
                                         ? HealthStatus.Healthy
                                         : HealthStatus.Unhealthy
                        };
                    }
                }
            }
            catch (Exception)
            {
                return new ApiHealthModel
                {
                    Name = site.Key.Name,
                    Url = site.Key.BaseAddress.AbsoluteUri,
                    Status = HealthStatus.Unhealthy
                };
            }
        }

        public async Task<IEnumerable<HealthModel>> GetDbHealthsAsync()
        {
            return await Task.WhenAll(_dbs.Select(CheckDbHealthAsync))
                             .ConfigureAwait(false);
        }

        private static async Task<HealthModel> CheckDbHealthAsync(DbConfig db)
        {
            try
            {
                using (var sqlConnection = new SqlConnection(db.ConnectionString))
                {
                    var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await sqlConnection.OpenAsync(cancellationToken.Token)
                                       .ConfigureAwait(false);
                }
                return new HealthModel
                {
                    Name = db.Name,
                    Status = HealthStatus.Healthy
                };
            }
            catch (Exception)
            {
                return new HealthModel
                {
                    Name = db.Name,
                    Status = HealthStatus.Unhealthy
                };
            }
        }


        public async Task<IEnumerable<ApiHealthModel>> GetApiHealthsAsync()
        {
            return await Task.WhenAll(_apis.Select(GetApiHealthAsync))
                             .ConfigureAwait(false);
        }

        private static async Task<ApiHealthModel> GetApiHealthAsync(KeyValuePair<ApiConfig, HttpClient> api)
        {
            api.ShouldNotBeNull();

            try
            {
                var response = JsonConvert.DeserializeObject<SiteHealthResponseModel>(await api.Value.GetStringAsync(api.Key.HealthEndpoint)
                                                                                               .ConfigureAwait(false));
                return new ApiHealthModel
                {
                    Name = api.Key.Name,
                    Url = api.Key.BaseAddress.AbsoluteUri,
                    Status = response.AvgResponseTime.Value <= api.Key.AvgResponseTimeThreshold
                                 ? HealthStatus.Healthy
                                 : HealthStatus.Struggling,
                    AvgResponseTime = response.AvgResponseTime.Value
                };

            }
            catch (Exception)
            {
                return new ApiHealthModel
                {
                    Name = api.Key.Name,
                    Url = api.Key.BaseAddress.AbsoluteUri,
                    Status = HealthStatus.Unhealthy
                };
            }
        }
    }
}