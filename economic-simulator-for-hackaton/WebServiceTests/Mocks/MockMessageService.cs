using economic_simulator_for_hackaton.Services.Interfaces;

namespace WebServiceTests.Mocks;

internal class MockMessageService : IMessageService
{
    public string GetResponse(Guid playerCharacterUuid, string text)
    {
        return "Hello world";
    }
}
