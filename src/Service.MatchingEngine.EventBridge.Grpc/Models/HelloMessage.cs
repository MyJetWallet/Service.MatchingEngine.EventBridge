using System.Runtime.Serialization;
using Service.MatchingEngine.EventBridge.Domain.Models;

namespace Service.MatchingEngine.EventBridge.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}