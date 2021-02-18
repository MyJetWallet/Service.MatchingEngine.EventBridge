using System.ServiceModel;
using System.Threading.Tasks;
using Service.MatchingEngine.EventBridge.Grpc.Models;

namespace Service.MatchingEngine.EventBridge.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}