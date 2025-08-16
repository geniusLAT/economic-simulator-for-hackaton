using Simulation.Entities.Characters;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Characters.CharacterTests;

public class LandTests
{
    private Simulator _simulator;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
    }


    [Test]
    public async Task Land_landed()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

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

        player.Place = ship;

        //Act
        var result = ship.Land(station);

        //Assert
       
        Assert.That(result, Is.True);
        Assert.That(ship.Parking, Is.EqualTo(station));
    }
}
