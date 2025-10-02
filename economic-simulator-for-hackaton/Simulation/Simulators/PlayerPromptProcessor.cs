using Simulation.Entities.Characters;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;

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
                return await ProcessViewCommand(player, words);
            case "сойти":
                return await ProcessDisEmbarkCommand(player, words);
            case "подняться":
                return await ProcessEmbarkCommand(player, words);
            case "посадка":
                return await ProcessLandCommand(player, words);
            case "взлёт":
                return await ProcessTakeOffCommand(player, words);
            case "взлет":
                return await ProcessTakeOffCommand(player, words);
            case "курс":
                return await ProcessSetDestinationCommand(player, words);
            case "разгрузить":
                return await ProcessUnloadCommand(player, words);
            case "загрузить":
                return await ProcessLoadCommand(player, words);
            case "торговать":
                return await ProcessTradeCommand(player, words);
            default:
                return "команда не распознана";
        }

        return "ошибка";
    }

    public async Task<string> ProcessSetDestinationCommand(PLayer pLayer, string[] words)
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
                return "Вы не капитан этого корабля, вы не можете отдавать приказ на смену курса";
            }

            if (ship.Parking is not null)
            {
                return "Корабль посажен и не может выставить курс";
            }
            if (words.Length < 3)
            {
                return "Вы не указали верные координаты, курс не задан";
            }

            uint potentialX;
            if(! uint.TryParse(words[1], out potentialX))
            {
                return $"{words[1]} не является корректной координатой";
            }

            uint potentialY;
            if (!uint.TryParse(words[2], out potentialY))
            {
                return $"{words[2]} не является корректной координатой";
            }
            ship.SetDestination(potentialX, potentialY);
            return $"Курс задан х = {ship.DestinationX}, y = {ship.DestinationY}";
        }
        return "Это не корабль, на котором можно выставить курс";
    }

    public async Task<string> ProcessTradeCommand(PLayer player, string[] words)
    {
        if (player.Place is null)
        {
            return "Вы находитесь нигде, тут не размещаются торговые предложения";
        }
        if (player.Place is SpaceShip)
        {
            return "Вы на корабле. На кораблях не размещаются торговые предложения";
        }
        if (player.Place is SpaceStation)
        {
            var station = player.Place as SpaceStation;

            if (words.Length < 2)
            {
                return "Вы не указали номер торгового предложения";
            }

            if (!int.TryParse(words[1], out int offerNumber))
            {
                return $"{words[1]} - некорректный номер торгового предложения";
            }

            if (offerNumber < 1)
            {
                return $"{words[1]} - некорректный номер торгового предложения, нумерация начинается с 1";
            }

            var trueOfferNumber = offerNumber - 1;
            if (trueOfferNumber >= station.localOffers.Count)
            {
                return $"торговое предложение с таким индексом не найдено, осмотритесь, чтобы найти индексы торговых предложений";
            }
            var offer = station.localOffers[trueOfferNumber];

            if (!int.TryParse(words[2], out int quantityToLoad))
            {
                return $"{words[2]} - некорректное количество груза для обмена";
            }

            if (quantityToLoad < 1)
            {
                return "Количество груза должно быть положительным";
            }

            if(quantityToLoad > offer.QuantityBorder)
            {
                return $"Контрагент не готов работать с настолько большим объёмом. Его максимум: {offer.QuantityBorder}";
            }

            if (!offer.IsOffererSelling)
            {
                Item? itemToSell = (from cargo in station.cargos
                                    where cargo.Owner == player && cargo.Type == offer.ItemType
                                    select cargo).FirstOrDefault();
                if (itemToSell is null)
                {
                    return "У вас нет груза, чтобы предложить его на продажу";
                }

                if (itemToSell.Quantity < quantityToLoad)
                {
                    return $"Нельзя продать {quantityToLoad} единиц груза, так как у вас есть только {itemToSell.Quantity}";
                }
            }
            else
            {
                var totalPrice = quantityToLoad * offer.pricePerOne;
                if (totalPrice > player.moneyBalance)
                {
                    return $"Нельзя купить {quantityToLoad} штук по цене {offer.pricePerOne}. " +
                        $"Это потребовало бы {totalPrice}, у вас только {player.moneyBalance}";
                }
            }

            if (offer.accept(player, quantityToLoad, station))
            {
                return "Сделка завершена";
            }
            else
            {
                return "Сделка не удалась";
            }
        }
        return "Тут не разгрузить";
    }

    public async Task<string> ProcessLoadCommand(PLayer player, string[] words)
    {
        if (player.Place is null)
        {
            return "Вы находитесь нигде, тут нет груза";
        }
        if (player.Place is SpaceShip )
        {
            return "Чтобы загружать корабль надо быть. Вы находитесь на корабле." +
                " У вас есть возможность рагружать";
        }
        if (player.Place is SpaceStation)
        {
            var station = player.Place as SpaceStation;

            if (words.Length < 2)
            {
                return "Вы не указали номер корабля для выгрузки";
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
                return $"корабль с таким индексом не найден, осмотритесь, чтобы найти индексы грузов";
            }
            var ship = station.parkedShips[trueShipNumber];

            if (!int.TryParse(words[2], out int cargoNumber))
            {
                return $"{words[2]} - некорректный номер груза";
            }

            if (cargoNumber < 1)
            {
                return $"{words[2]} - некорректный номер груза, нумерация начинается с 1";
            }

            var trueCargoNumber = cargoNumber - 1;
            if (trueCargoNumber >= station.cargos.Count)
            {
                return $"груз с таким индексом не найден, осмотритесь, чтобы найти индексы грузов";
            }

            if (words.Length == 3)
            {
                if (player.Load(station.cargos[trueCargoNumber], ship))
                {

                    return $"Груз загружен";
                }
                else
                {
                    return $"Загрузить груз не удалось";
                }
            }

            if (!int.TryParse(words[3], out int quantityToLoad))
            {
                return $"{words[3]} - некорректное количество груза";
            }

            if (quantityToLoad < 1)
            {
                return "Количество груза должно быть положительным";
            }

            if (quantityToLoad > station.cargos[trueCargoNumber].Quantity)
            {
                return $"Количество груза на cтанции равно {station.cargos[trueCargoNumber].Quantity}, " +
                    $"нельзя зарузить  {quantityToLoad} единиц, это больше чем имеется на cтанции";
            }

            if (player.Load(station.cargos[trueCargoNumber], ship, (uint)quantityToLoad))
            {
                return $"Груз загружен";
            }
            else
            {
                return $"Загрузить груз не удалось";
            }
        }
        return "Тут не разгрузить";
    }

    public async Task<string> ProcessUnloadCommand(PLayer player, string[] words)
    {
        if (player.Place is null)
        {
            return "Вы находитесь нигде, тут нет груза";
        }
        if (player.Place is SpaceStation)
        {
            return "Чтобы разгружать корабль надо подняться на его борт. Вы находитесь на станции." +
                " У вас есть возможность загружать";
        }
        if (player.Place is SpaceShip)
        {
            var ship = player.Place as SpaceShip;

            if (ship.Parking is null)
            {
                return "Корабль не припаркован, вы не можете разгружать в полёте, нужна посадка";
            }

            var station = ship.Parking as SpaceStation; 
            if (words.Length < 2)
            {
                return "Вы не указали номер груза для выгрузки";
            }

            if (!int.TryParse(words[1], out int cargoNumber))
            {
                return $"{words[1]} - некорректный номер груза";
            }

            if (cargoNumber < 1)
            {
                return $"{words[1]} - некорректный номер груза, нумерация начинается с 1";
            }

            var trueCargoNumber = cargoNumber - 1;
            if (trueCargoNumber >= ship.cargos.Count)
            {
                return $"груз с таким индексом не найден, осмотритесь, чтобы найти индексы грузов";
            }

            if (words.Length == 2)
            {
                if (player.Unload(ship.cargos[trueCargoNumber]))
                {
                
                    return $"Груз разгружен";
                }
                else
                {
                    return $"Разгрузить груз не удалось";
                }
            }

            if (!int.TryParse(words[2], out int quantityToUnload))
            {
                return $"{words[2]} - некорректное количество груза";
            }

            if (quantityToUnload < 1)
            {
                return "Количество груза должно быть положительным";
            }

            if (quantityToUnload > ship.cargos[trueCargoNumber].Quantity)
            {
                return $"Количество груза на корабле равно {ship.cargos[trueCargoNumber].Quantity}, " +
                    $"нельзя выгрузить  {quantityToUnload} единиц, это больше чем имеется на борту";
            }

            if (player.Unload(ship.cargos[trueCargoNumber], (uint)quantityToUnload))
            {
                return $"Груз разгружен";
            }
            else
            {
                return $"Разгрузить груз не удалось";
            }
        }
        return "Тут не разгрузить";
    }

    public async Task<string> ProcessViewCommand(PLayer player, string[] words)
    {
        if (words.Length == 1)
        {
            Console.WriteLine("Only one word");
            return player.Place?.View() ?? "Пустота и ничего более"; 
        }

        if (words[1] == "груз")
        {
            return player.Place?.CargoView() ?? "Пустота и ничего более";
        }

        return "ошибка команды осмотра";
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
