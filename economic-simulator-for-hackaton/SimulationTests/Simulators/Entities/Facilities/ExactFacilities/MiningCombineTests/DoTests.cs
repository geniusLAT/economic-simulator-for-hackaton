using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Facilities.ExactFacilities.MiningCombineTests;

public class DoTests
{
    private Simulator _simulator;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
    }

    [Test]
    public async Task Do_SetFacilityBehavior()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var ceoBehavior = new CeoBehavior();

        var character = new Character()
        {
            Name = "Joe Doe",
            Behavior = ceoBehavior,
            Place = station
        };
        _simulator.Characters.Add(character);

        var MiningCombine = new MiningCombine()
        {
            Name = "Zeus Mining",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(MiningCombine);
        station.facilities.Add(MiningCombine);

        //Act
        await _simulator.SkipDays(20);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(MiningCombine.Behavior, Is.Not.Null);
        Assert.That(station.cargos.Count, Is.EqualTo(1));
        Assert.That(station.cargos.First().Quantity, Is.EqualTo(3));
        Assert.That(station.cargos.First().Owner, Is.EqualTo(MiningCombine));
    }

    [Test]
    public async Task Do_SellsALot_WantsToScaleUp()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var ceoBehavior = new CeoBehavior();

        var character = new Character()
        {
            Name = "Joe Doe",
            Behavior = ceoBehavior,
            Place = station
        };
        _simulator.Characters.Add(character);

        var MiningCombine = new MiningCombine()
        {
            Name = "Zeus Mining",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(MiningCombine);
        station.facilities.Add(MiningCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.ore },
            Place = station,
            moneyBalance = 100000
        };
        _simulator.Characters.Add(Buyer);

        var Speculator = new Character()
        {
            Name = "German",
            Behavior = new SpeculatorBehavior(),
            Place = station,
            moneyBalance = 100000
        };
        _simulator.Characters.Add(Speculator);

        //Act
        await _simulator.SkipDays(20);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(MiningCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(MiningCombine.moneyBalance, Is.Positive);
        Assert.That(
            station.localOffers
            .Where(offer => offer.ItemType == ItemType.miningEquipment)
            .Count(), 
            Is.EqualTo(1)
        );
    }


    [Test]
    public async Task Do_SellsALot_ScalesUp()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var ceoBehavior = new CeoBehavior();

        var character = new Character()
        {
            Name = "Joe Doe",
            Behavior = ceoBehavior,
            Place = station
        };
        _simulator.Characters.Add(character);

        var MiningCombine = new MiningCombine()
        {
            Name = "Zeus Mining",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(MiningCombine);
        station.facilities.Add(MiningCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.ore },
            Place = station,
            moneyBalance = 100000
        };
        _simulator.Characters.Add(Buyer);

        var Seller = new Character()
        {
            Name = "Fred",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };
        _simulator.Characters.Add(Seller);

        var miningEqipment = new Item()
        {
            Type = ItemType.miningEquipment,
            Owner = Seller,
            Quantity = 1
        };
        station.cargos.Add(miningEqipment);

        var Speculator = new Character()
        {
            Name = "German",
            Behavior = new SpeculatorBehavior(),
            Place = station,
            moneyBalance = 100000
        };
        _simulator.Characters.Add(Speculator);

        //Act
        await _simulator.SkipDays(20);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(MiningCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(MiningCombine.moneyBalance, Is.Positive);
        Assert.That(MiningCombine.Level, Is.GreaterThan(1));
    }
}
