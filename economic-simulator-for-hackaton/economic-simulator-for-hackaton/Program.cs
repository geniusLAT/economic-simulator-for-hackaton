using economic_simulator_for_hackaton.Controllers;
using economic_simulator_for_hackaton.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMessageService, MessageService>();

var app = builder.Build();

app.MapPost("/api/message", (HttpRequest request, string text, IMessageService service) =>
{
    if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
    {
        return Results.Unauthorized();
    }

    if (!Guid.TryParse(authHeader, out Guid userGuid))
    {
        return Results.Unauthorized();
    }

    var resp = service.GetResponse(userGuid, text);
    return Results.Ok(new { response = resp });
});


app.Run();
