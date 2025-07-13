using AdminByRequestChallange.API;
using AdminByRequestChallenge.API.Middlewares;
using AdminByRequestChallenge.Core;
using AdminByRequestChallenge.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// For production. Add json file through Azure Key Vault.
//builder.Configuration.AddAzureKeyVault(vaultUri, new DefaultAzureCredential());
if (!string.IsNullOrWhiteSpace(env))
{
    builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);
}

//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.UseCore(builder.Configuration);
builder.Services.UseDbContexts(builder.Configuration);
RSA rsa = RSA.Create();

// Hack to get keys
//RSAParameters rsaKeyInfo = rsa.ExportParameters(false);
//var publicKey = System.Convert.ToBase64String(rsa.ExportRSAPublicKey());
//var privateKey= System.Convert.ToBase64String(rsa.ExportRSAPrivateKey());

var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWTSettings>();

RSA publicRsa = RSA.Create();
publicRsa.ImportRSAPublicKey(
    source: Convert.FromBase64String(jwtSettings.PublicKey),
    bytesRead: out int _
);

var signingKey = new RsaSecurityKey(publicRsa);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = !string.IsNullOrWhiteSpace(jwtSettings.Issuer),
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(jwtSettings.Audience),
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build());

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Admin By Request Challenge API",
        Version = "v1"
    });

    // Define the header-based SessionKey scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Apply globally to all endpoints (unless [AllowAnonymous])
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "text/json", "application/json; charset=utf-8", "text/json; charset=utf-8" });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<SessionValidatorMiddleware>();
app.UseMiddleware<RateLimiterMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
