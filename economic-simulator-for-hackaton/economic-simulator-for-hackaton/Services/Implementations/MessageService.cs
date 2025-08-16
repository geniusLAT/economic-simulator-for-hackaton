using economic_simulator_for_hackaton.Services.Interfaces;

namespace economic_simulator_for_hackaton.Controllers;

public class MessageService : IMessageService
{
    public string GetResponse(Guid playerCharacterUuid, string text)
    {

        return $"Received: {text}";
    }
}
