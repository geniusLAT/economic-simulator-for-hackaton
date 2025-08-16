using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Characters.CharacterTests;

public class DoTests
{
    private Simulator _simulator;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
    }


    [Test]
    public async Task Do_undloadedEverythingOwned()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var character = new Character()
        {
            Name = "Joe Doe",
            Behavior = new StupidSellerBehavior()
        };

        _simulator.Characters.Add(character);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = character,
            Owner = character,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);

        var Cargo = new Item()
        {
            Owner = character,
            Type = ItemType.food,

        };
        ship.cargos.Add(Cargo);

        character.Place = ship;

        //Act
        await _simulator.FinishDay();

        //Assert
        Assert.That(ship.cargos, Is.Empty);
        Assert.That(station.cargos.Contains(Cargo), Is.True);
        Assert.That(character.Place, Is.EqualTo(station));
    }

    [Test]
    public async Task Do_DecreasingPricesWithoutSelling()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var character = new Character()
        {
            Name = "Joe Doe",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };

        _simulator.Characters.Add(character);

        var Cargo = new Item()
        {
            Owner = character,
            Type = ItemType.food,

        };
        station.cargos.Add(Cargo);

        //Act
        await _simulator.FinishDay();
        var firstPrice = station.localOffers.First().pricePerOne;
        await _simulator.FinishDay();
        var secondPrice = station.localOffers.First().pricePerOne;

        //Assert
        Assert.That(secondPrice, Is.EqualTo(firstPrice - (0.1f * firstPrice)));
    }

    [Test]
    public async Task Do_IncreasingPricesWhileSelling()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var character = new Character()
        {
            Name = "Joe Doe",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };

        _simulator.Characters.Add(character);

        var Cargo = new Item()
        {
            Owner = character,
            Type = ItemType.food,

        };
        station.cargos.Add(Cargo);

        //Act
        await _simulator.FinishDay();
        var theOffer = station.localOffers.First();
        var firstPrice = theOffer.pricePerOne;
        theOffer.WasUsedYesterday = 10;
        await _simulator.FinishDay();
        var secondPrice = station.localOffers.First().pricePerOne;

        //Assert
        var ExpectedSecondPrice = firstPrice;
        Console.WriteLine($"Expected first price change: {ExpectedSecondPrice}");
        ExpectedSecondPrice += ExpectedSecondPrice * 0.1f;
        Console.WriteLine($"Expected second price change: {ExpectedSecondPrice}");
        Assert.That(secondPrice, Is.EqualTo(ExpectedSecondPrice));
    }
}
