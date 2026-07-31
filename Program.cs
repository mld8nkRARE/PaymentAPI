using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PaymentAPI.Application.Auth;
using PaymentAPI.Application.Orders;
using PaymentAPI.Application.Payments;
using PaymentAPI.Application.Refunds;
using PaymentAPI.Application.Webhook;
using PaymentAPI.Domain;
using PaymentAPI.Domain.Payments;
using PaymentAPI.Domain.Refunds;
using PaymentAPI.Infrastructure;
using PaymentAPI.Primitives;
using PaymentAPI.Providers.Interfaces;
using PaymentAPI.Providers.Yookassa;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DomainEventPublishingInterceptor>();
builder.Services.AddDbContext<ApplicationDbContext>((sp,options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention()
    .AddInterceptors(sp.GetRequiredService<DomainEventPublishingInterceptor>());
});

builder.Services.AddIdentity<User, IdentityRole<UserId>>(options => 
{
    options.Password.RequiredLength = 12;
    options.Password.RequireDigit = true;
    options.Password.RequiredUniqueChars = 4;
    options.Password.RequireNonAlphanumeric = true;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var jwtSettiings = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettiings["iss"],
            ValidAudience = jwtSettiings["aud"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettiings["key"]))
        };
        options.RequireHttpsMetadata = false;
        options.MapInboundClaims = false;
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});
builder.Services.Configure<YookassaSettings>(builder.Configuration.GetSection("YookassaSettings"));
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPaymentGateway<PaymentCreateYookassaCommand>, YookassaPaymentGateway>();
builder.Services.AddScoped<IRefundGateway<RefundCreateYookassaCommand>, YookassaRefundGateway>();
builder.Services.AddScoped<PaymentHandler>();
builder.Services.AddScoped<WebhookHandler>();
builder.Services.AddScoped<WebhookVerifierContext>();
builder.Services.AddScoped<ISourceIpVerifier, YookassaIpVerifier>();
builder.Services.AddScoped<RefundHandler>();
//builder.Services.AddHostedService<RefundPollingService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IWebhookClassifier,YookassaWebhookClassifier>();
builder.Services.AddScoped<IPaymentWebhookHandler, YookassaPaymentWebhookHandler>();
builder.Services.AddScoped<OrderService>();

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "PaymentAPI",
        Version = "v1",
        Description = "Этот API предоставляет endpoints для управления товарами, заказами и пользователями. Поддерживает аутентификацию через JWT.",
    });
    options.IncludeXmlComments(xmlPath);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT-токен в формате: Bearer {token}",

    });
    options.AddSecurityRequirement(document =>  new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecuritySchemeReference("Bearer"),
                new List<string>()
            }
    });
});


builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseForwardedHeaders();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
