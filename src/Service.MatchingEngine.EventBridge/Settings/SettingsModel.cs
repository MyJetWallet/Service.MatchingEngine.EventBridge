using SimpleTrading.SettingsReader;

namespace Service.MatchingEngine.EventBridge.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        public SettingsModel()
        {
            RabbitMqExchange = "spot.me.events";
        }

        [YamlProperty("MatchingEngineEventBridge.SeqServiceUrl")] public string SeqServiceUrl { get; set; }

        [YamlProperty("MatchingEngineEventBridge.ServiceBus.HostPort")]  public string ServiceBusHostPort { get; set; }
        [YamlProperty("MatchingEngineEventBridge.ServiceBus.TopicName")] public string TopicName { get; set; }

        [YamlProperty("MatchingEngineEventBridge.RabbitMq.ConnectionString")]  public string RabbitMqConnectionString { get; set; }
        [YamlProperty("MatchingEngineEventBridge.RabbitMq.Exchange")]   public string RabbitMqExchange { get; set; }
        [YamlProperty("MatchingEngineEventBridge.RabbitMq.QueryName")]  public string RabbitMqQueryName { get; set; }
        [YamlProperty("MatchingEngineEventBridge.ZipkinUrl")] public string ZipkinUrl { get; set; }
    }
}