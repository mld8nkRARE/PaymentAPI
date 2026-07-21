using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaymentAPI.Interfaces;
using PaymentAPI.Settings;
using System.Security.Cryptography;
using System.Text;

namespace PaymentAPI.Services
{
    public class YookassaSignatureVerifier : ISignatureVerifier
    {
        private readonly string _shopId;
        private readonly ECDsa _ecdsa;
        public string ProviderName => "yookassa";
        public YookassaSignatureVerifier(IOptions<YookassaSettings> options)
        {
            _shopId = options.Value.ShopId;
            _ecdsa = LoadYookassaPublicKey(options.Value.PublicKeyBase64);
        }
        public async Task<bool> VerifySignatureAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            if (!request.Headers.TryGetValue("Signature", out var signatureValue) || string.IsNullOrEmpty(signatureValue.FirstOrDefault()))
            {
                throw new UnauthorizedAccessException("Заголовок Signature отсутствует или пустой");
            }

            string[] signature = signatureValue.ToString().Split(' ');
            byte[] payloadBytes = FormPayload(request,rawBody,signature);
            string signatureBase64 = signature[3];
            byte[] signatureByte = Convert.FromBase64String(signatureBase64);
            return _ecdsa.VerifyData(payloadBytes, signatureByte, HashAlgorithmName.SHA384, DSASignatureFormat.Rfc3279DerSequence);

            
        }
        private ECDsa LoadYookassaPublicKey(string publicKeyBase64)
        {
            var pemBytes = Convert.FromBase64String(publicKeyBase64);
            string pemString = Encoding.UTF8.GetString(pemBytes);
            var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(pemString);
            return ecdsa;
        }
        private byte[] FormPayload(HttpRequest request, string rawBody, string[] signature)
        {
            if (signature.Length != 4)
            {
                throw new UnauthorizedAccessException("Неверный формат подписи");
            }
            string version = signature[0];
            string timestamp = signature[1];
            string httpMethod = request.Method;
            string endpointUrl = request.GetEncodedUrl();
            string idempotenceKey = request.Headers.TryGetValue("Idempotence-Key", out var idempotenceKeyValue)
                ? idempotenceKeyValue.ToString() : "";

            if (version != "v1")
            {
                throw new UnauthorizedAccessException("Неверный параметр version");
            }

            string payload = $"{timestamp}\n{httpMethod}\n{endpointUrl}\n{_shopId}\n{idempotenceKey}\n{rawBody}";
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            return payloadBytes;
        }
    }
}
