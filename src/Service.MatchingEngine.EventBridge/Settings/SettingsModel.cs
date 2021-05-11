using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.MatchingEngine.EventBridge.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MatchingEngineEventBridge.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MatchingEngineEventBridge.ServiceBus.HostPort")]
        public string ServiceBusHostPort { get; set; }

        [YamlProperty("MatchingEngineEventBridge.ServiceBus.TopicName")]
        public string TopicName { get; set; }

        [YamlProperty("MatchingEngineEventBridge.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MatchingEngineEventBridge.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
    }
}