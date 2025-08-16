using static System.Net.Mime.MediaTypeNames;
using economic_simulator_for_hackaton.Controllers;
using economic_simulator_for_hackaton.Services.Interfaces;

namespace economic_simulator_for_hackaton;

public class WebService
{
    public async Task ExecuteAsync(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IMessageService, MessageService>();

        var app = builder.Build();

        app.MapPost("/api/message", async (HttpRequest request, IMessageService service) =>
        {
            if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
            {
                return Results.Unauthorized();
            }

            if (!Guid.TryParse(authHeader, out Guid userGuid))
            {
                return Results.Unauthorized();
            }

            using var reader = new StreamReader(request.Body);
            var bodyString = await reader.ReadToEndAsync();

            var resp = service.GetResponse(userGuid, bodyString);
            return Results.Ok(new { response = resp });
        });


        app.Run();
    }
}
