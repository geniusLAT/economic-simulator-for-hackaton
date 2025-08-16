using Simulation.Entities;
using Simulation.Entities.Characters;
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
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r","")));
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
Торговых предложений: 0
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
}
