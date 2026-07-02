using PaymentAPI.Interfaces;
using PaymentAPI.Services;
using PaymentAPI.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<YookassaSettings>(builder.Configuration.GetSection("YookassaSettings"));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPaymentGateway, YookassaGateway>();

var app = builder.Build();


app.MapControllers();

app.Run();
