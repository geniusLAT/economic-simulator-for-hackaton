using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Characters.BehaviorModel.StupidBuyerBehaviorTests;

public class DoTests
{
    private Simulator _simulator;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
    }


    [Test]
    public async Task Do_EmbarkedAndPublished()
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
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.food }
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

        character.Place = ship;

        //Act
        await _simulator.FinishDay();

        //Assert
        Assert.That(character.Place, Is.EqualTo(station));
        Assert.That(station.localOffers.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task Do_IncreasingPricesWithoutBuing()
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
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.food },
            Place = station
        };

        _simulator.Characters.Add(character);

        //var Cargo = new Item()
        //{
        //    Owner = character,
        //    Type = ItemType.food,

        //};
        //station.cargos.Add(Cargo);

        //Act
        await _simulator.FinishDay();
        var firstPrice = station.localOffers.First().pricePerOne;
        await _simulator.FinishDay();
        var secondPrice = station.localOffers.First().pricePerOne;

        //Assert
        Assert.That(secondPrice, Is.EqualTo(firstPrice + 0.1f * firstPrice));
    }

    [Test]
    public async Task Do_DecreasingPricesWhileBuying()
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
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.food },
            Place = station
        };

        _simulator.Characters.Add(character);

        //var Cargo = new Item()
        //{
        //    Owner = character,
        //    Type = ItemType.food,

        //};
        //station.cargos.Add(Cargo);

        //Act
        await _simulator.FinishDay();
        var theOffer = station.localOffers.First();
        var firstPrice = theOffer.pricePerOne;
        theOffer.WasUsedYesterday = 1;
        await _simulator.FinishDay();
        var secondPrice = station.localOffers.First().pricePerOne;

        //Assert
        var ExpectedSecondPrice = firstPrice;
        Console.WriteLine($"Expected first price change: {ExpectedSecondPrice}");
        ExpectedSecondPrice -= ExpectedSecondPrice * 0.1f;
        Console.WriteLine($"Expected second price change: {ExpectedSecondPrice}");
        Assert.That(secondPrice, Is.EqualTo(ExpectedSecondPrice));
    }
}
