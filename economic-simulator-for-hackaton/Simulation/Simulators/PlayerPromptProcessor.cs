using System;

namespace Simulation.Simulators;

public class PlayerPromptProcessor
{
    private readonly Simulator _simulator;

    public PlayerPromptProcessor(Simulator simulator)
    {
        _simulator = simulator;
    }

    public async Task<string> ProcessPromptAsync(string prompt, Guid PlayerGuid)
    {
        var player = _simulator.PLayerCharacters.FirstOrDefault(p => p.Guid == PlayerGuid);
        if (player == null)
        {
            return "Вы мертвы или не существуете";
        }

        switch (prompt)
        {
            case "помощь":
                break;
            case "осмотр":
                return player.Place?.View() ?? "Пустота и ничего более"; 
            default:
                return "команда не распознана";
        }

        return "ошибка";
    }
}
