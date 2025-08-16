using Simulation.Entities.Characters;
using Simulation.Entities.Locations;
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
        string[] words = prompt.Split(' ');
        switch (words[0])
        {
            case "помощь":
                break;
            case "осмотр":
                return player.Place?.View() ?? "Пустота и ничего более";
            case "сойти":
                return await ProcessDisEmbarkCommand(player, words);
            default:
                return "команда не распознана";
        }

        return "ошибка";
    }

    public async Task<string> ProcessDisEmbarkCommand(PLayer pLayer, string[] words)
    {
        if(pLayer.Place is null)
        {
            return "Вы находитесь нигде, вам некуда сойти  и не с чего";
        }
        if (pLayer.Place is SpaceStation)
        {
            return "Вы уже находитесь на станции, у вас нет возможности сойти с корабля, пока вы не на корабле";
        }
        if (pLayer.Place is SpaceShip)
        {
            var ship = pLayer.Place as SpaceShip;
            if (ship.Parking is not null) 
            {
                if (pLayer.Disembark())
                {
                    return $"Вы успешно сошли на станцию {pLayer.Place.Name}";
                }
                else
                {
                    return $"Не удалось сойти на станцию";
                }
            }else
            {
                return "Ваш корабль не сел на поверхность станции, он в открытом космосе, сойти невозможно";
            }    
        }
        return "Тут не сойти";
    }
}
