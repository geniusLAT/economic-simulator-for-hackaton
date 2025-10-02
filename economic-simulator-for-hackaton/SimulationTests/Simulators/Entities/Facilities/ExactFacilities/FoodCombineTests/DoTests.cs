using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Facilities.ExactFacilities.FoodCombineTests;

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

        var FoodCombine = new FoodCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FoodCombine);
        station.facilities.Add(FoodCombine);

        //Act
        await _simulator.SkipDays(20);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(FoodCombine.Behavior, Is.Not.Null);
        Assert.That(station.cargos.Count, Is.EqualTo(1));
        Assert.That(station.cargos.First().Quantity, Is.EqualTo(3));
        Assert.That(station.cargos.First().Owner, Is.EqualTo(FoodCombine));
    }

    [Test]
    public async Task Do_SellsALot_WantsToScaleUp()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II",
            IsSunny = true,
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

        var FoodCombine = new FoodCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FoodCombine);
        station.facilities.Add(FoodCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.food },
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
        Assert.That(FoodCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(FoodCombine.moneyBalance, Is.Positive);
        Assert.That(
            station.localOffers
            .Where(offer => offer.ItemType == ItemType.farmingEquipment)
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
            Name = "Zeus II",
            IsSunny = true
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

        var FoodCombine = new FoodCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FoodCombine);
        station.facilities.Add(FoodCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.food },
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

        var farmingEquipment = new Item()
        {
            Type = ItemType.farmingEquipment,
            Owner = Seller,
            Quantity = 1
        };
        station.cargos.Add(farmingEquipment);

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
        Assert.That(FoodCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(FoodCombine.moneyBalance, Is.Positive);
        Assert.That(FoodCombine.Level, Is.GreaterThan(1));
    }

    [Test]
    public async Task Do_SellsALot_ScalesUpAsMuchAsCan()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II",
            IsSunny = true
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

        var FoodCombine = new FoodCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FoodCombine);
        station.facilities.Add(FoodCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.food },
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

        var farmingEquipment = new Item()
        {
            Type = ItemType.farmingEquipment,
            Owner = Seller,
            Quantity = 10
        };
        station.cargos.Add(farmingEquipment);

        var Speculator = new Character()
        {
            Name = "German",
            Behavior = new SpeculatorBehavior(),
            Place = station,
            moneyBalance = 100000
        };
        _simulator.Characters.Add(Speculator);

        //Act
        await _simulator.SkipDays(400);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(FoodCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(FoodCombine.moneyBalance, Is.Positive);
        Assert.That(FoodCombine.Level, Is.GreaterThan(2));
    }
}
