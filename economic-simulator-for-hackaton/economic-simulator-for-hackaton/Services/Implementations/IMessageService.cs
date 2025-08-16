using economic_simulator_for_hackaton.Services.Interfaces;

namespace economic_simulator_for_hackaton.Controllers;

public class MessageService : IMessageService
{
    public string GetResponse(string text)
    {

        return $"Принято: {text}";
    }
}
