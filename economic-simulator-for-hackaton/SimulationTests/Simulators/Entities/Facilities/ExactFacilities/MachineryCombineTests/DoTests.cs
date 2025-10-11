using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Facilities.ExactFacilities.MachineryCombineTests;

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

        var machineryCombine = new MachineryCombine()
        {
            Name = "Zeus Machinery",
            Place = station,
            Ceo = character,
            Owner = character,
            moneyBalance = 1000
        };
        ceoBehavior.myFacilities.Add(machineryCombine);
        station.facilities.Add(machineryCombine);

        var metal = new Item()
        {
            Type = ItemType.metal,
            Owner = machineryCombine,
            Quantity = 30
        };
        station.cargos.Add(metal);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = machineryCombine,
            Quantity = 30
        };
        station.cargos.Add(fuel);
        //Act
        await _simulator.SkipDays(20);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(machineryCombine.Behavior, Is.Not.Null);

        var machinery = station.cargos.Where(cargo => cargo.Type == ItemType.farmingEquipment).FirstOrDefault();

        Assert.That(machinery, Is.Not.Null);
        Assert.That(machinery.Quantity, Is.EqualTo(1));
        Assert.That(machinery.Owner, Is.EqualTo(machineryCombine));
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

        var machineryCombine = new MachineryCombine()
        {
            Name = "Zeus Machinery",
            Place = station,
            Ceo = character,
            Owner = character,
        };
        ceoBehavior.myFacilities.Add(machineryCombine);
        station.facilities.Add(machineryCombine);

        var metal = new Item()
        {
            Type = ItemType.metal,
            Owner = machineryCombine,
            Quantity = 2000
        };
        station.cargos.Add(metal);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = machineryCombine,
            Quantity = 2000
        };
        station.cargos.Add(fuel);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.farmingEquipment },
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
        Assert.That(machineryCombine.Behavior, Is.Not.Null);
        Assert.That(Buyer.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(machineryCombine.moneyBalance, Is.Positive);
        Assert.That(
            station.localOffers
            .Where(offer => offer.ItemType == ItemType.machineryProducingEquipment)
            .Count(), 
            Is.EqualTo(1)
        );
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

        var machineryCombine = new MachineryCombine()
        {
            Name = "Zeus Machinery",
            Place = station,
            Ceo = character,
            Owner = character,
            moneyBalance = 500
        };
        ceoBehavior.myFacilities.Add(machineryCombine);
        station.facilities.Add(machineryCombine);

        var Seller = new Character()
        {
            Name = "Fred",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };
        _simulator.Characters.Add(Seller);

        var metal = new Item()
        {
            Type = ItemType.metal,
            Owner = Seller,
            Quantity = 20000
        };
        station.cargos.Add(metal);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = Seller,
            Quantity = 2000
        };
        station.cargos.Add(fuel);

        var Buyer = new Character()
        {
            Name = "Linda",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.farmingEquipment },
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
        await _simulator.SkipDays(400);

        //Assert
        Console.WriteLine(station.CargoView());
        Console.WriteLine(station.View());
        Assert.That(machineryCombine.Behavior, Is.Not.Null);
        Assert.That(Seller.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(machineryCombine.moneyBalance, Is.Positive);

        var sellerCargos = station.cargos.Where(cargo => cargo.Owner == Seller);

        Assert.That(sellerCargos.Count(), Is.LessThan(3));
    }

    [Test]
    public async Task Do_SellsALot_MakesALotofMoneyBecauseThereAreTwoBuyers()
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

        var machineryCombine = new MachineryCombine()
        {
            Name = "Zeus Machinery",
            Place = station,
            Ceo = character,
            Owner = character,
            moneyBalance = 500
        };
        ceoBehavior.myFacilities.Add(machineryCombine);
        station.facilities.Add(machineryCombine);

        var Seller = new Character()
        {
            Name = "Fred",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };
        _simulator.Characters.Add(Seller);

        var metal = new Item()
        {
            Type = ItemType.metal,
            Owner = Seller,
            Quantity = 20000
        };
        station.cargos.Add(metal);

        var fuel = new Item()
        {
            Type = ItemType.fuel,
            Owner = Seller,
            Quantity = 2000
        };
        station.cargos.Add(fuel);

        for (int i = 0; i < 2; i++)
        {
            var Buyer = new Character()
            {
                Name = $"Linda {i+1}",
                Behavior = new StupidBuyerBehavior()
                { TypeToBuy = ItemType.farmingEquipment },
                Place = station,
                moneyBalance = 100000
            };
            _simulator.Characters.Add(Buyer);
        }
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
        Assert.That(machineryCombine.Behavior, Is.Not.Null);
        Assert.That(Seller.moneyBalance, Is.LessThan(100000));
        Assert.That(Speculator.moneyBalance, Is.GreaterThan(100000));
        Assert.That(machineryCombine.moneyBalance, Is.Positive);

        var sellerCargos = station.cargos.Where(cargo => cargo.Owner == Seller);

        Assert.That(sellerCargos.Count(), Is.LessThan(3));
    }
}
