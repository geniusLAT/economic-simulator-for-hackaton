namespace economic_simulator_for_hackaton.Services.Interfaces;

public interface IMessageService
{
    string GetResponse(Guid playerCharacterUuid, string text);
}