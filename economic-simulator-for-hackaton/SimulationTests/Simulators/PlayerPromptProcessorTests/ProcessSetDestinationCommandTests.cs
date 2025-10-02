using Simulation.Entities.Characters;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.PlayerPromptProcessorTests;

public class ProcessSetDestinationCommandTests
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
    public async Task ProcessPrompt_SetDestinationSucessfully()
    {
        //Append
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс 10 20", pLayer.Guid);

        //Assert
        var expected = "Курс задан х = 10, y = 20";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(10));
        Assert.That(ship.DestinationY, Is.EqualTo(20));
    }

    [Test]
    public async Task ProcessPrompt_SetDestinationNowhereInsteadOfShip()
    {
        //Append
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс 10 20", pLayer.Guid);

        //Assert
        var expected = "Вы находитесь нигде, не на корабле";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(0));
        Assert.That(ship.DestinationY, Is.EqualTo(0));
    }

    [Test]
    public async Task ProcessPrompt_SetDestinationAtStationInsteadOfShip()
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс 10 20", pLayer.Guid);

        //Assert
        var expected = "Вы находитесь на станции, станции не летают";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(0));
        Assert.That(ship.DestinationY, Is.EqualTo(0));
    }

    [Test]
    public async Task ProcessPrompt_LandByNotCaptainInsteadOfShip()
    {
        //Append
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс 10 20", pLayer.Guid);

        //Assert
        var expected = "Вы не капитан этого корабля, вы не можете отдавать приказ на смену курса";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(0));
        Assert.That(ship.DestinationY, Is.EqualTo(0));
    }

    [Test]
    public async Task ProcessPrompt_SetDestinationWithNoArgument()
    {
        //Append
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс", pLayer.Guid);

        //Assert
        var expected = "Вы не указали верные координаты, курс не задан";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(0));
        Assert.That(ship.DestinationY, Is.EqualTo(0));
    }

    [Test]
    public async Task ProcessPrompt_SetDestinationWithTheOnlyArgument()
    {
        //Append
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс 10", pLayer.Guid);

        //Assert
        var expected = "Вы не указали верные координаты, курс не задан";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(0));
        Assert.That(ship.DestinationY, Is.EqualTo(0));
    }


    [Test]
    public async Task ProcessPrompt_SetDestinationWithNegativeX()
    {
        //Append
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс -10 20", pLayer.Guid);

        //Assert
        var expected = "-10 не является корректной координатой";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(0));
        Assert.That(ship.DestinationY, Is.EqualTo(0));
    }

    [Test]
    public async Task ProcessPrompt_SetDestinationWithNegativeY()
    {
        //Append
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("курс 10 -20", pLayer.Guid);

        //Assert
        var expected = "-20 не является корректной координатой";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.DestinationX, Is.EqualTo(0));
        Assert.That(ship.DestinationY, Is.EqualTo(0));
    }
}
