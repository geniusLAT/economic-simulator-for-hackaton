using Simulation.Entities;
using Simulation.Entities.Characters;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.PlayerPromptProcessorTests;

public class ProcessPromptTests
{
    private Simulator _simulator;

    private PlayerPromptProcessor _playerPromptProcessor;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
        _playerPromptProcessor = new(_simulator);
    }

    [Test]
    public async Task ProcessPrompt_PlayerDoesNotExist()
    {
        //Append
        
        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("помощь", Guid.NewGuid());

        //Assert
        Assert.That(result, Is.EqualTo("Вы мертвы или не существуете"));
    }

    [Test]
    public async Task ProcessPrompt_PlayerExistsButWrongCommand()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("ABCDEF", pLayer.Guid);

        //Assert
        Assert.That(result, Is.EqualTo("команда не распознана"));
    }

    [Test]
    public async Task ProcessPrompt_PlayerViewNowhere()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            //Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр", pLayer.Guid);

        //Assert
        Assert.That(result, Is.EqualTo("Пустота и ничего более"));
    }

    [Test]
    public async Task ProcessPrompt_PlayerViewAtStation()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Станция называется Zeus II, находится по координатам 0, 0
Уровень пригодности для добычи руды: 0
Уровень пригодности для добычи топлива: 0
Пригодность для синтеза еды: Нет
Торговых предложений: 0
Кораблей припарковано: 0
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r","")));
    }

    [Test]
    public async Task ProcessPrompt_PlayerCargoViewAtEmptyStation()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр груз 3", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Грузов: 0";
        Assert.That(result, Is.EqualTo(expected.Replace("\r", "")));
    }

    [Test]
    public async Task ProcessPrompt_PlayerCargoViewStationWithThreeCargos()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var cargo1 = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo1.TransitToNewLocation(null, station);

        var cargo2 = new Item()
        {
            Owner = pLayer,
            Quantity = 20,
            Type = ItemType.food
        };
        cargo2.TransitToNewLocation(null, station);

        var cargo3 = new Item()
        {
            Owner = pLayer,
            Quantity = 100,
            Type = ItemType.metal
        };
        cargo3.TransitToNewLocation(null, station);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр груз", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Грузов: 130
|Номер|Наименование товара|Количество|Владелец|
|-----|-------------------|----------|--------|
|1    |Руда               |10        |Joe Doe |
|2    |Продовольствие     |20        |Joe Doe |
|3    |Металл             |100       |Joe Doe |
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r", "")));
    }

    [Test]
    public async Task ProcessPrompt_PlayerViewAtStationWithOffers()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var offer = new Offer()
        { 
            Offerer = pLayer,
            IsOffererSelling = false,
            ItemType = Simulation.Entities.Items.ItemType.food,
            pricePerOne = 10
        };
        station.localOffers.Add(offer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Станция называется Zeus II, находится по координатам 0, 0
Уровень пригодности для добычи руды: 0
Уровень пригодности для добычи топлива: 0
Пригодность для синтеза еды: Нет
Торговых предложений: 1
|Номер|Наименование товара|вид предложения|цена за штуку|верхний предел товара|Автор предложения|
|-----|-------------------|---------------|-------------|---------------------|-----------------|
|1    |Продовольствие     |скупает        |10           |1                    |Joe Doe          |
Кораблей припарковано: 0
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r", "")));
    }


    [Test]
    public async Task ProcessPrompt_PlayerViewAtStationWithOffersAndShips()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var offer = new Offer()
        {
            Offerer = pLayer,
            IsOffererSelling = false,
            ItemType = Simulation.Entities.Items.ItemType.food,
            pricePerOne = 10
        };
        station.localOffers.Add(offer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);

        station.parkedShips.Add(ship);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Станция называется Zeus II, находится по координатам 0, 0
Уровень пригодности для добычи руды: 0
Уровень пригодности для добычи топлива: 0
Пригодность для синтеза еды: Нет
Торговых предложений: 1
|Номер|Наименование товара|вид предложения|цена за штуку|верхний предел товара|Автор предложения|
|-----|-------------------|---------------|-------------|---------------------|-----------------|
|1    |Продовольствие     |скупает        |10           |1                    |Joe Doe          |
Кораблей припарковано: 1
|Номер|Название судна|Владелец|Капитан|
|-----|--------------|--------|-------|
|1    |Pegasus       |Joe Doe |Joe Doe|
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r", "")));
    }

    [Test]
    public async Task ProcessPrompt_PlayerViewAtSpaceShip()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe"
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Корабль называется Pegasus, находится по координатам 0, 0
Владелец: Joe Doe
Капитан: Joe Doe
Посажен на станции Zeus II
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r", "")));
    }

    [Test]
    public async Task ProcessPrompt_PlayerFacilityViewStationWithFiveFacilities()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var f1 = new MiningCombine()
        {
            Owner = pLayer,
            Ceo = pLayer,
            Place = station
        };
        station.facilities.Add(f1);

        var f2 = new FoodCombine()
        {
            Owner = pLayer,
            Ceo = pLayer,
            Place = station
        };
        station.facilities.Add(f2);

        var f3 = new MeltingCombine()
        {
            Owner = pLayer,
            Ceo = pLayer,
            Place = station
        };
        station.facilities.Add(f3);

        var f4 = new FuelCombine()
        {
            Owner = pLayer,
            Ceo = pLayer,
            Place = station
        };
        station.facilities.Add(f4);

        var f5 = new MiningCombine()
        {
            Owner = pLayer,
            Ceo = pLayer,
            Place = station
        };
        station.facilities.Add(f5);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр предприятия", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Предприятий основано: 5
|Номер|Название |Владелец|Директор|Род деятельности       |
|-----|---------|--------|--------|-----------------------|
|1    |Новое Имя|Joe Doe |Joe Doe |Шахта                  |
|2    |Новое Имя|Joe Doe |Joe Doe |Продуктовый комбинат   |
|3    |Новое Имя|Joe Doe |Joe Doe |Рудоплавильный комбинат|
|4    |Новое Имя|Joe Doe |Joe Doe |Топливный комбинат     |
|5    |Новое Имя|Joe Doe |Joe Doe |Шахта                  |
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r", "")));
    }
}
