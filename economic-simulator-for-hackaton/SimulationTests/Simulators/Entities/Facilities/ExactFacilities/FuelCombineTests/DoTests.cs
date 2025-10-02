using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Facilities.ExactFacilities.FuelCombineTests;

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

        var FuelCombine = new FuelCombine()
        {
            Name = "Zeus Mining",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FuelCombine);
        station.facilities.Add(FuelCombine);

        //Act
        await _simulator.SkipDays(20);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(FuelCombine.Behavior, Is.Not.Null);
        Assert.That(station.cargos.Count, Is.EqualTo(1));
        Assert.That(station.cargos.First().Quantity, Is.EqualTo(3));
        Assert.That(station.cargos.First().Owner, Is.EqualTo(FuelCombine));
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
            MaxLevelOfFuel = 2
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

        var FuelCombine = new FuelCombine()
        {
            Name = "Zeus Mining",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FuelCombine);
        station.facilities.Add(FuelCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.fuel },
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
        Assert.That(FuelCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(FuelCombine.moneyBalance, Is.Positive);
        Assert.That(
            station.localOffers
            .Where(offer => offer.ItemType == ItemType.fuelProducingEquipment)
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
            MaxLevelOfFuel = 2
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

        var FuelCombine = new FuelCombine()
        {
            Name = "Zeus Mining",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FuelCombine);
        station.facilities.Add(FuelCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.fuel },
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

        var fuelProducingEquipment = new Item()
        {
            Type = ItemType.fuelProducingEquipment,
            Owner = Seller,
            Quantity = 1
        };
        station.cargos.Add(fuelProducingEquipment);

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
        Assert.That(FuelCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(FuelCombine.moneyBalance, Is.Positive);
        Assert.That(FuelCombine.Level, Is.GreaterThan(1));
    }

    [Test]
    public async Task Do_SellsALot_ScalesUpTillBottom()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II",
            MaxLevelOfFuel = 2
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

        var FuelCombine = new FuelCombine()
        {
            Name = "Zeus Mining",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(FuelCombine);
        station.facilities.Add(FuelCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.fuel },
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

        var fuelProducingEquipment = new Item()
        {
            Type = ItemType.fuelProducingEquipment,
            Owner = Seller,
            Quantity = 10
        };
        station.cargos.Add(fuelProducingEquipment);

        var Speculator = new Character()
        {
            Name = "German",
            Behavior = new SpeculatorBehavior(),
            Place = station,
            moneyBalance = 100000
        };
        _simulator.Characters.Add(Speculator);

        //Act
        await _simulator.SkipDays(200);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(FuelCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(FuelCombine.moneyBalance, Is.Positive);
        Assert.That(FuelCombine.Level, Is.EqualTo(2));
    }
}
