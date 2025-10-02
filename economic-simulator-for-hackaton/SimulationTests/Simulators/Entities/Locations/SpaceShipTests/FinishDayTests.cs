using Simulation.Entities.Characters;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Characters.CharacterTests;

public class FinishDayTests
{
    private Simulator _simulator;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
    }


    [Test]
    public async Task FinishDay_CoordinatesChangedForHalfWay()
    {
        //Append
        var player = new PLayer()
        {
            Name = "Joe Doe"
        };

        _simulator.Characters.Add(player);
        _simulator.PLayerCharacters.Add(player);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = player,
            Owner = player,
            Parking = null
        };
        _simulator.spaceShips.Add(ship);
        ship.SetDestination(10, 20);

        player.Place = ship;

        //Act
        for (var i = 0; i < 10; i++)
        {
            await _simulator.FinishDay();
        }
        //Assert
        Assert.That(ship.coordX, Is.EqualTo(10));
        Assert.That(ship.coordY, Is.EqualTo(10));
    }

    [Test]
    public async Task FinishDay_CoordinatesChangedForFullWay()
    {
        //Append
        var player = new PLayer()
        {
            Name = "Joe Doe"
        };

        _simulator.Characters.Add(player);
        _simulator.PLayerCharacters.Add(player);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = player,
            Owner = player,
            Parking = null
        };
        _simulator.spaceShips.Add(ship);
        ship.SetDestination(10, 20);

        player.Place = ship;

        //Act
        for (var i = 0; i < 20; i++)
        {
            await _simulator.FinishDay();
        }
        //Assert
        Assert.That(ship.coordX, Is.EqualTo(10));
        Assert.That(ship.coordY, Is.EqualTo(20));
    }

    [Test]
    public async Task FinishDay_CoordinatesChangedForFullWayNegativeWay()
    {
        //Append
        var player = new PLayer()
        {
            Name = "Joe Doe"
        };

        _simulator.Characters.Add(player);
        _simulator.PLayerCharacters.Add(player);

        var ship = new SpaceShip()
        {
            coordX = 10,
            coordY = 20,
            Name = "Pegasus",
            Captain = player,
            Owner = player,
            Parking = null
        };
        _simulator.spaceShips.Add(ship);
        ship.SetDestination(0, 0);

        player.Place = ship;

        //Act
        for (var i = 0; i < 20; i++)
        {
            await _simulator.FinishDay();
        }
        //Assert
        Assert.That(ship.coordX, Is.EqualTo(0));
        Assert.That(ship.coordY, Is.EqualTo(0));
    }

    [Test]
    public async Task FinishDay_CoordinatesChangedForFullWayNegativeWayWaitedMore()
    {
        //Append
        var player = new PLayer()
        {
            Name = "Joe Doe"
        };

        _simulator.Characters.Add(player);
        _simulator.PLayerCharacters.Add(player);

        var ship = new SpaceShip()
        {
            coordX = 10,
            coordY = 20,
            Name = "Pegasus",
            Captain = player,
            Owner = player,
            Parking = null
        };
        _simulator.spaceShips.Add(ship);
        ship.SetDestination(0, 0);

        player.Place = ship;

        //Act
        for (var i = 0; i < 40; i++)
        {
            await _simulator.FinishDay();
        }
        //Assert
        Assert.That(ship.coordX, Is.EqualTo(0));
        Assert.That(ship.coordY, Is.EqualTo(0));
    }
}
