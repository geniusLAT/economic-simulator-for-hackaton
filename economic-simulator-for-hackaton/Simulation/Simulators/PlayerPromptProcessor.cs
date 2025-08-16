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
            case "посадка":
                return await ProcessLandCommand(player, words);
            case "взлёт":
                return await ProcessTakeOffCommand(player, words);
            case "взлет":
                return await ProcessTakeOffCommand(player, words);
            default:
                return "команда не распознана";
        }

        return "ошибка";
    }

    public async Task<string> ProcessDisEmbarkCommand(PLayer pLayer, string[] words)
    {
        if (pLayer.Place is null)
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
            }
            else
            {
                return "Ваш корабль не сел на поверхность станции, он в открытом космосе, сойти невозможно";
            }
        }
        return "Тут не сойти";
    }

    public async Task<string> ProcessEmbarkCommand(PLayer pLayer, string[] words)
    {
        if (pLayer.Place is null)
        {
            return "Вы находитесь нигде, вам некуда сойти  и не с чего";
        }
        if (pLayer.Place is SpaceShip)
        {
            return "Вы уже находитесь на корабле, у вас нет возможности подняться на корабля, пока вы на корабле";
        }
        if (pLayer.Place is SpaceStation)
        {
            var station = pLayer.Place as SpaceStation;
            if(words.Length <2)
            {
                return "Вы не указали номер корабля для подъёма";
            }

            if (!int.TryParse(words[1], out int shipNumber))
            {
                return $"{words[1]} - некорректный номер корабля";
            }

            if (shipNumber < 1)
            {
                return $"{words[1]} - некорректный номер корабля, нумерация начинается с 1";
            }

            var trueShipNumber = shipNumber - 1;
            if (trueShipNumber >= station.parkedShips.Count)
            {
                return $"Корабль с таким индексом не найден, осмотритесь, чтобы найти индексы кораблей";
            }

            if (pLayer.Embark(station.parkedShips[trueShipNumber]))
            {
                return $"Вы поднимаетесь на борт {station.parkedShips[trueShipNumber].Name}";
            }
            else
            {
                return $"Подняться на борт не удалось";
            }
        }
        return "Тут не сойти";
    }

    public async Task<string> ProcessLandCommand(PLayer pLayer, string[] words)
    {
        if (pLayer.Place is null)
        {
            return "Вы находитесь нигде, не на корабле";
        }
        if (pLayer.Place is SpaceStation)
        {
            return "Вы находитесь на станции, станции не летают";
        }
        if (pLayer.Place is SpaceShip)
        {
            var ship = pLayer.Place as SpaceShip;
            if (ship.Captain != pLayer)
            {
                return "Вы не капитан этого корабля, вы не можете отдавать приказ на посадку";
            }

            if (ship.Parking is not null)
            {
                return "Корабль уже посажен";
            }

            var station = _simulator.GetStationByCoord(ship.coordX, ship.coordY);
            if (station is null)
            {
                return "Рядом нет станции для посадки";
            }
            if (ship.Land(station))
            {
                return "Посадка завершена успешно";

            }
            return "Посадка не удалась";
        }
        return "Это не корабль, на котором можно влететь";
    }

    public async Task<string> ProcessTakeOffCommand(PLayer pLayer, string[] words)
    {
        if (pLayer.Place is null)
        {
            return "Вы находитесь нигде, не на корабле";
        }
        if (pLayer.Place is SpaceStation)
        {
            return "Вы находитесь на станции, станции не летают";
        }
        if (pLayer.Place is SpaceShip)
        {
            var ship = pLayer.Place as SpaceShip;
            if (ship.Captain != pLayer)
            {
                return "Вы не капитан этого корабля, вы не можете отдавать приказ на взлёт";
            }

            if (ship.Parking is null)
            {
                return "Корабль не посажен, он не может взлететь";
            }

            if (ship.TakeOff())
            {
                return "Взлёт завершен успешно";

            }
            return "Взлёт не удался";
        }
        return "Это не корабль, на котором можно взлететь";
    }
}
