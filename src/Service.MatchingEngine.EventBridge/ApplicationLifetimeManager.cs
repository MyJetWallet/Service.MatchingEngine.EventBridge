using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.MatchingEngine.EventReader;
using MyJetWallet.Sdk.Service;
using MyServiceBus.TcpClient;

namespace Service.MatchingEngine.EventBridge
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyServiceBusTcpClient _serviceBusTcpClient;
        private readonly MatchingEngineGlobalEventReader _globalEventReader;

        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger,
            MyServiceBusTcpClient serviceBusTcpClient,
            MatchingEngineGlobalEventReader globalEventReader)
            : base(appLifetime)
        {
            _logger = logger;
            _serviceBusTcpClient = serviceBusTcpClient;
            _globalEventReader = globalEventReader;
        }

        protected override void OnStarted()
        {
            _serviceBusTcpClient.Start();
            _globalEventReader.Start();
            _logger.LogInformation("OnStarted has been called.");
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _globalEventReader.Stop();
            _serviceBusTcpClient.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
