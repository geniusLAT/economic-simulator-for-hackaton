using Simulation.Entities.Characters;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.PlayerPromptProcessorTests;

public class ProcessLandCommandTests
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
    public async Task ProcessPrompt_LandedSucessfully()
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
            Parking = null
        };
        _simulator.spaceShips.Add(ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("посадка", pLayer.Guid);

        //Assert
        var expected = "Посадка завершена успешно";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.EqualTo(station));
    }

    [Test]
    public async Task ProcessPrompt_LandedNowhereInsteadOfShip()
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
            Parking = null
        };
        _simulator.spaceShips.Add(ship);

        pLayer.Place = null;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("посадка", pLayer.Guid);

        //Assert
        var expected = "Вы находитесь нигде, не на корабле";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.Not.EqualTo(station));
    }

    [Test]
    public async Task ProcessPrompt_LandAtStationInsteadOfShip()
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
            Parking = null
        };
        _simulator.spaceShips.Add(ship);

        pLayer.Place = station;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("посадка", pLayer.Guid);

        //Assert
        var expected = "Вы находитесь на станции, станции не летают";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.Not.EqualTo(station));

    }

    [Test]
    public async Task ProcessPrompt_LandByNotCaptainInsteadOfShip()
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
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = null,
            Owner = pLayer,
            Parking = null
        };
        _simulator.spaceShips.Add(ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("посадка", pLayer.Guid);

        //Assert
        var expected = "Вы не капитан этого корабля, вы не можете отдавать приказ на посадку";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.Not.EqualTo(station));
    }

    [Test]
    public async Task ProcessPrompt_LandOutOfStation()
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
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 5,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = null
        };
        _simulator.spaceShips.Add(ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("посадка", pLayer.Guid);

        //Assert
        var expected = "Рядом нет станции для посадки";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.Not.EqualTo(station));
    }

    [Test]
    public async Task ProcessPrompt_LandAlreadyLanded()
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("посадка", pLayer.Guid);

        //Assert
        var expected = "Корабль уже посажен";
        Assert.That(result, Is.EqualTo(expected));
    }
}
