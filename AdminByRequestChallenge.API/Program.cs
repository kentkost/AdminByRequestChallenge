using AdminByRequestChallenge.API;
using AdminByRequestChallenge.Core;
using AdminByRequestChallenge.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// For production. Add json file through Azure Key Vault.
// Or get configuration through some Repository/Store
if (!string.IsNullOrWhiteSpace(env))
{
    builder.Configuration.AddJsonFile($"appsettings.{env}.json", true, true);
}

builder.Services.AddSingleton<ISessionStore, InMemorySessionStore>();
builder.Services.AddSingleton<IJwtFactory, JwtFactory>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.UseCore(builder.Configuration);
builder.Services.UseDbContexts(builder.Configuration);

builder.Services.AddAuthentication(SessionKeyAuthenticationOptions.DefaultScheme)
                .AddScheme<SessionKeyAuthenticationOptions, SessionKeyAuthenticationHandler>(SessionKeyAuthenticationOptions.DefaultScheme, opt => {  });

builder.Services.AddAuthorization(opts =>
{
    opts.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(SessionKeyAuthenticationOptions.DefaultScheme)
        .RequireAuthenticatedUser()
        .Build();
});

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
    c.AddSecurityDefinition("SessionKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "X-Session-Key",              // This matches what your handler reads
        In = ParameterLocation.Header,
        Description = "Paste your session-key here"
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
                    Id = "SessionKey"
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

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
