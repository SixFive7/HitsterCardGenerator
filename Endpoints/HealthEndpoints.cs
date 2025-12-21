namespace HitsterCardGenerator.Endpoints;

/// <summary>
/// Health check endpoints for monitoring and readiness probes
/// </summary>
public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/health");

        // Basic health check
        group.MapGet("", () => new
        {
            status = "ok",
            timestamp = DateTime.UtcNow
        })
        .WithName("GetHealth");

        // Readiness check
        group.MapGet("/ready", () => new
        {
            ready = true
        })
        .WithName("GetReadiness");
    }
}
