using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using MyServiceBus.TcpClient;

namespace Service.MatchingEngine.EventBridge
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyServiceBusTcpClient _serviceBusTcpClient;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime,
            ILogger<ApplicationLifetimeManager> logger,
            MyServiceBusTcpClient serviceBusTcpClient)
            : base(appLifetime)
        {
            _logger = logger;
            _serviceBusTcpClient = serviceBusTcpClient;
        }

        protected override void OnStarted()
        {
            _serviceBusTcpClient.Start();
            _logger.LogInformation("OnStarted has been called.");
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _serviceBusTcpClient.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}