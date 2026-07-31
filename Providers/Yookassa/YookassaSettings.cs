namespace PaymentAPI.Providers.Yookassa
{
    public class YookassaSettings
    {
        public string ShopId { get; set; } = String.Empty;
        public string SecretKey { get; set; } = String.Empty;
        public string ReturnUrl { get; set; } = String.Empty;
        public string[] AllowedWebhooksIPs { get; set; } = Array.Empty<string>();
    }
}
