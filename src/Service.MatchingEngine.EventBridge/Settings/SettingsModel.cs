using SimpleTrading.SettingsReader;

namespace Service.MatchingEngine.EventBridge.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("MatchingEngine.EventBridge.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }
    }
}