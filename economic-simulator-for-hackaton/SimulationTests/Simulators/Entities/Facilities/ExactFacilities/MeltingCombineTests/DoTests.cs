using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Facilities.ExactFacilities.MeltingCombineTests;

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

        var MeltingCombine = new MeltingCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
            moneyBalance = 1000
        };
        ceoBehavior.myFacilities.Add(MeltingCombine);
        station.facilities.Add(MeltingCombine);

        var ore = new Item()
        {
            Type = ItemType.ore,
            Owner = MeltingCombine,
            Quantity = 20
        };
        station.cargos.Add(ore);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = MeltingCombine,
            Quantity = 20
        };
        station.cargos.Add(fuel);
        //Act
        await _simulator.SkipDays(20);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(MeltingCombine.Behavior, Is.Not.Null);

        var metal = station.cargos.Where(cargo => cargo.Type == ItemType.metal).FirstOrDefault();

        Assert.That(metal, Is.Not.Null);
        Assert.That(metal.Quantity, Is.EqualTo(3));
        Assert.That(metal.Owner, Is.EqualTo(MeltingCombine));
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

        var MeltingCombine = new MeltingCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(MeltingCombine);
        station.facilities.Add(MeltingCombine);

        var ore = new Item()
        {
            Type = ItemType.ore,
            Owner = MeltingCombine,
            Quantity = 2000
        };
        station.cargos.Add(ore);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = MeltingCombine,
            Quantity = 2000
        };
        station.cargos.Add(fuel);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.metal },
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
        await _simulator.SkipDays(60);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(MeltingCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(MeltingCombine.moneyBalance, Is.Positive);
        Assert.That(
            station.localOffers
            .Where(offer => offer.ItemType == ItemType.meltingEquipment)
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

        var MeltingCombine = new MeltingCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(MeltingCombine);
        station.facilities.Add(MeltingCombine);

        var ore = new Item()
        {
            Type = ItemType.ore,
            Owner = MeltingCombine,
            Quantity = 2000
        };
        station.cargos.Add(ore);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = MeltingCombine,
            Quantity = 2000
        };
        station.cargos.Add(fuel);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.metal },
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

        var meltingEquipment = new Item()
        {
            Type = ItemType.meltingEquipment,
            Owner = Seller,
            Quantity = 1
        };
        station.cargos.Add(meltingEquipment);

        var Speculator = new Character()
        {
            Name = "German",
            Behavior = new SpeculatorBehavior(),
            Place = station,
            moneyBalance = 100000
        };
        _simulator.Characters.Add(Speculator);

        //Act
        await _simulator.SkipDays(60);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(MeltingCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(MeltingCombine.moneyBalance, Is.Positive);
        Assert.That(MeltingCombine.Level, Is.GreaterThan(1));
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

        var MeltingCombine = new MeltingCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(MeltingCombine);
        station.facilities.Add(MeltingCombine);

        var ore = new Item()
        {
            Type = ItemType.ore,
            Owner = MeltingCombine,
            Quantity = 2000
        };
        station.cargos.Add(ore);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = MeltingCombine,
            Quantity = 2000
        };
        station.cargos.Add(fuel);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.metal },
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

        var meltingEquipment = new Item()
        {
            Type = ItemType.meltingEquipment,
            Owner = Seller,
            Quantity = 10
        };
        station.cargos.Add(meltingEquipment);

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
        Assert.That(MeltingCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(MeltingCombine.moneyBalance, Is.Positive);
        Assert.That(MeltingCombine.Level, Is.GreaterThan(2));
    }

    [Test]
    public async Task Do_SellsALot_BuysRawMaterials()
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

        var MeltingCombine = new MeltingCombine()
        {
            Name = "Zeus Farming",
            Place = station,
            Ceo = character,
            Owner = character,
            moneyBalance = 100
        };
        ceoBehavior.myFacilities.Add(MeltingCombine);
        station.facilities.Add(MeltingCombine);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.metal },
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

        var meltingEquipment = new Item()
        {
            Type = ItemType.meltingEquipment,
            Owner = Seller,
            Quantity = 10
        };
        station.cargos.Add(meltingEquipment);

        var ore = new Item()
        {
            Type = ItemType.ore,
            Owner = Seller,
            Quantity = 20000
        };
        station.cargos.Add(ore);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = Seller,
            Quantity = 2000
        };
        station.cargos.Add(fuel);

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
        Assert.That(MeltingCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(MeltingCombine.moneyBalance, Is.Positive);
        Assert.That(MeltingCombine.Level, Is.GreaterThan(10));

        var sellerCargos = station.cargos.Where(cargo => cargo.Owner == Seller);

        Assert.That(sellerCargos.Count(), Is.LessThan(3));
    }
}
