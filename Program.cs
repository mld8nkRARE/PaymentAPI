using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Infrastructure;
using PaymentAPI.Interfaces;
using PaymentAPI.Services;
using PaymentAPI.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention();
});
builder.Services.Configure<YookassaSettings>(builder.Configuration.GetSection("YookassaSettings"));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPaymentGateway, YookassaGateway>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseRouting();

app.MapControllers();

app.Run();
