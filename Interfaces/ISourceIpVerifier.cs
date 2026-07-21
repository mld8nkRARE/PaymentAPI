using PaymentAPI.DTO;
using System.Runtime;
using System.Text.Json;

namespace PaymentAPI.Interfaces
{
    public interface ISourceIpVerifier
    {
        string ProviderName { get; }
        bool VerifySourceIp(HttpContext httpContext);
    }
}
