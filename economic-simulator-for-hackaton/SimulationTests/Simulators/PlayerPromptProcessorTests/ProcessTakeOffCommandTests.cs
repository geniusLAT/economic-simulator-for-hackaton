using Simulation.Entities.Characters;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.PlayerPromptProcessorTests;

public class ProcessTakeOffCommandTests
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
    public async Task ProcessTakeOffCommand_TakenOffSuccessfully_AlternativeCommand()
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("взлет", pLayer.Guid);

        //Assert
        var expected = "Взлёт завершен успешно";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.Not.EqualTo(station));
    }

    [Test]
    public async Task ProcessTakeOffCommand_TakenOffSuccessfully()
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("взлёт", pLayer.Guid);

        //Assert
        var expected = "Взлёт завершен успешно";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.Not.EqualTo(station));
    }

    [Test]
    public async Task ProcessTakeOffCommand_TakenOffFailed_PlayerNowhere()
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

        pLayer.Place = null;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("взлёт", pLayer.Guid);

        //Assert
        var expected = "Вы находитесь нигде, не на корабле";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.EqualTo(station));
    }

    [Test]
    public async Task ProcessTakeOffCommand_TakenOffFailed_PlayerAtTheStation()
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

        pLayer.Place = station;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("взлёт", pLayer.Guid);

        //Assert
        var expected = "Вы находитесь на станции, станции не летают";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.EqualTo(station));
    }

    [Test]
    public async Task ProcessTakeOffCommand_TakenOffFailed_PlayerIsNotCaptain()
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
            Parking = station
        };
        _simulator.spaceShips.Add(ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("взлёт", pLayer.Guid);

        //Assert
        var expected = "Вы не капитан этого корабля, вы не можете отдавать приказ на взлёт";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.EqualTo(station));
    }

    [Test]
    public async Task ProcessTakeOffCommand_TakenOffFailed_ShipIsNotParked()
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
        var result = await _playerPromptProcessor.ProcessPromptAsync("взлёт", pLayer.Guid);

        //Assert
        var expected = "Корабль не посажен, он не может взлететь";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.Parking, Is.Not.EqualTo(station));
    }
}
