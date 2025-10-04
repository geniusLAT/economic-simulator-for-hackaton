using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Characters.BehaviorModel.EnthusiastBehaviorTests;

public class DoTests
{
    private Simulator _simulator;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
    }


    [Test]
    public async Task Do_StartedUp()
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

        var enthusiast = new Character()
        {
            Name = "Harisson",
            Behavior = new EnthusiastBehavior(),
            Place = station,
            moneyBalance = 500
        };

        var cargo = new Item()
        {
            Owner = enthusiast,
            Type = ItemType.farmingEquipment,
            Quantity = 1
        };
        cargo.TransitToNewLocation(null, station);

        _simulator.Characters.Add(enthusiast);

        await _simulator.FinishDay();

        //Act       
        await _simulator.FinishDay();

        //Assert
        Assert.That(station.facilities.Count, Is.EqualTo(1));
        Assert.That(station.facilities.FirstOrDefault().Owner, Is.EqualTo(enthusiast));
    }

    [Test]
    public async Task Do_StartedUpAndSoldUselessCargo()
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

        var enthusiast = new Character()
        {
            Name = "Harisson",
            Behavior = new EnthusiastBehavior(),
            Place = station,
            moneyBalance = 500
        };

        var cargo = new Item()
        {
            Owner = enthusiast,
            Type = ItemType.farmingEquipment,
            Quantity = 1
        };
        cargo.TransitToNewLocation(null, station);

        var uselessCargo = new Item()
        {
            Owner = enthusiast,
            Type = ItemType.ore,
            Quantity = 50
        };
        uselessCargo.TransitToNewLocation(null, station);

        _simulator.Characters.Add(enthusiast);

        await _simulator.FinishDay();

        //Act       
        await _simulator.FinishDay();

        //Assert
        Assert.That(station.facilities.Count, Is.EqualTo(1));
        Assert.That(station.facilities.FirstOrDefault().Owner, Is.EqualTo(enthusiast));
        Assert.That(station.localOffers
            .FirstOrDefault(offer => offer.ItemType == ItemType.ore),
            Is.Not.Null);
    }

    [Test]
    public async Task Do_BoughtEquipmentAndStartedUp()
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

        var enthusiast = new Character()
        {
            Name = "Harisson",
            Behavior = new EnthusiastBehavior(),
            Place = station,
            moneyBalance = 500
        };

        var seller = new Character()
        {
            Name = "seller I",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };
        _simulator.Characters.Add(seller);

        var cargo = new Item()
        {
            Owner = seller,
            Type = ItemType.farmingEquipment,
            Quantity = 1
        };
        cargo.TransitToNewLocation(null, station);

        var uselessCargo = new Item()
        {
            Owner = seller,
            Type = ItemType.miningEquipment,
            Quantity = 1
        };
        uselessCargo.TransitToNewLocation(null, station);

        _simulator.Characters.Add(enthusiast);

        await _simulator.FinishDay();

        //Act       
        await _simulator.FinishDay();

        //Assert
        Assert.That(station.facilities.Count, Is.EqualTo(1));
        Assert.That(station.facilities.FirstOrDefault().Owner, Is.EqualTo(enthusiast));
    }

    [Test]
    public async Task Do_BoughtBestEquipmentAndStartedUp()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II",
            MaxLevelOfMining = 3
        };

        _simulator.spaceStations.Add(station);

        var enthusiast = new Character()
        {
            Name = "Harisson",
            Behavior = new EnthusiastBehavior(),
            Place = station,
            moneyBalance = 500
        };

        var seller = new Character()
        {
            Name = "seller I",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };
        _simulator.Characters.Add(seller);

        var cargo = new Item()
        {
            Owner = seller,
            Type = ItemType.farmingEquipment,
            Quantity = 1
        };
        cargo.TransitToNewLocation(null, station);

        var uselessCargo = new Item()
        {
            Owner = seller,
            Type = ItemType.miningEquipment,
            Quantity = 1
        };
        uselessCargo.TransitToNewLocation(null, station);

        _simulator.Characters.Add(enthusiast);

        await _simulator.FinishDay();

        //Act       
        await _simulator.FinishDay();

        //Assert
        Assert.That(station.facilities.Count, Is.EqualTo(1));
        Assert.That(station.facilities.FirstOrDefault().Owner, Is.EqualTo(enthusiast));
        Assert.That(station.facilities.FirstOrDefault() is MiningCombine);
    }

    [Test]
    public async Task Do_ChoosesLessChallengedBusinessAndStartedUp()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II",
            MaxLevelOfMining = 3,
            MaxLevelOfFuel = 3
        };

        _simulator.spaceStations.Add(station);

        var enthusiast = new Character()
        {
            Name = "Harisson",
            Behavior = new EnthusiastBehavior(),
            Place = station,
            moneyBalance = 500
        };

        var seller = new Character()
        {
            Name = "seller I",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };
        _simulator.Characters.Add(seller);

        var cargo = new Item()
        {
            Owner = seller,
            Type = ItemType.fuelProducingEquipment,
            Quantity = 1
        };
        cargo.TransitToNewLocation(null, station);

        var uselessCargo = new Item()
        {
            Owner = seller,
            Type = ItemType.miningEquipment,
            Quantity = 1
        };
        uselessCargo.TransitToNewLocation(null, station);

        _simulator.Characters.Add(enthusiast);

        var miningFacility = new MiningCombine()
        { 
            Place = station,
            Owner = seller
        };
        station.facilities.Add(miningFacility);
        await _simulator.FinishDay();

        //Act       
        await _simulator.FinishDay();

        //Assert
        Assert.That(station.facilities.Count, Is.EqualTo(2));
        Assert.That(station.facilities.Last().Owner, Is.EqualTo(enthusiast));
        Assert.That(station.facilities.Last() is FuelCombine);
    }

    [Test]
    public async Task Do_ChoosesPotentialGrowthBusinessAndStartedUp()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II",
            MaxLevelOfMining = 30,
            MaxLevelOfFuel = 1
        };

        _simulator.spaceStations.Add(station);

        var enthusiast = new Character()
        {
            Name = "Harisson",
            Behavior = new EnthusiastBehavior(),
            Place = station,
            moneyBalance = 500
        };

        var seller = new Character()
        {
            Name = "seller I",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };
        _simulator.Characters.Add(seller);

        var cargo = new Item()
        {
            Owner = seller,
            Type = ItemType.fuelProducingEquipment,
            Quantity = 1
        };
        cargo.TransitToNewLocation(null, station);

        var uselessCargo = new Item()
        {
            Owner = seller,
            Type = ItemType.miningEquipment,
            Quantity = 1
        };
        uselessCargo.TransitToNewLocation(null, station);

        _simulator.Characters.Add(enthusiast);

        var miningFacility = new MiningCombine()
        {
            Place = station,
            Owner = seller
        };
        station.facilities.Add(miningFacility);
        await _simulator.FinishDay();

        //Act       
        await _simulator.FinishDay();

        //Assert
        Assert.That(station.facilities.Count, Is.EqualTo(2));
        Assert.That(station.facilities.Last().Owner, Is.EqualTo(enthusiast));
        Assert.That(station.facilities.Last() is MiningCombine);
    }
}
