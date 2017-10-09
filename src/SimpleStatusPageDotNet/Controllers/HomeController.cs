using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using SimpleStatusPageDotNet.Configuration;
using SimpleStatusPageDotNet.Models;

namespace SimpleStatusPageDotNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppOptions _appOptions;
        private readonly IHealthCheckHelper _healthCheckHelper;

        public HomeController(ILogger<HomeController> logger,
                              IOptions<AppOptions> options,
                              IHealthCheckHelper healthCheckHelper)
        {
            logger.ShouldNotBeNull();
            options.ShouldNotBeNull();
            healthCheckHelper.ShouldNotBeNull();

            _logger = logger;
            _appOptions = options.Value;
            _healthCheckHelper = healthCheckHelper;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogTrace(nameof(Index));

            var siteHealthChecksTask = _healthCheckHelper.GetSiteHealthsAsync();
            var apiHealthChecksTask = _healthCheckHelper.GetApiHealthsAsync();
            var dbHealthCheckTask = _healthCheckHelper.GetDbHealthsAsync();

            await Task.WhenAll(siteHealthChecksTask, apiHealthChecksTask, dbHealthCheckTask)
                      .ConfigureAwait(false);

            var model = new StatusPageModel
            {
                AppName = _appOptions.AppName,
                AppLogoUrl = _appOptions.AppLogoUrl,
                Sites = siteHealthChecksTask.Result,
                Apis = apiHealthChecksTask.Result,
                KPIs = new[]
                {
                    new KPIModel
                    {
                        Name = "Some internal system",
                        Status = HealthStatus.Healthy,
                        LastActivityOn = "5 seconds ago"
                    }
                },
                ThirdPartyServices = new[]
                {
                    new HealthModel
                    {
                        Name = "Some third party service",
                        Status = HealthStatus.Healthy
                    }
                },
                DBs = dbHealthCheckTask.Result
            };

            if (Request.Query.ContainsKey("h")) // testing
            {
                model.Apis.Last().AvgResponseTime = 10000;
                model.Apis.Last().Status = HealthStatus.Struggling;
            }

            Response.StatusCode = model.IsOkOverall
                                      ? (int) HttpStatusCode.OK
                                      : (int) HttpStatusCode.ServiceUnavailable;

            return View(model);
        }
    }
}