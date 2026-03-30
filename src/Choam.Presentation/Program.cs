using System.Text.Json.Serialization;
using Choam.Application;
using Choam.Infrastructure;
using Choam.Infrastructure.Data;
using Choam.Presentation.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// --- Layer DI Registration ---
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --- ASP.NET Core Services ---
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

// --- Middleware Pipeline ---

// Respect X-Forwarded-* headers from Caddy reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Central exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
