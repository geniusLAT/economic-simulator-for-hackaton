using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.Entities.Characters.BehaviorModel.SpeculatorBehaviorTests;

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

        var buyer = new Character()
        {
            Name = "buyer I",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.food },
            Place = station,
            moneyBalance = 500
        };

        var seller = new Character()
        {
            Name = "seller I",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };

        var cargo = new Item()
        {
            Owner = seller,
            Type = ItemType.food,
            Quantity = 7
        };
        cargo.TransitToNewLocation(null, station);


        _simulator.Characters.Add(buyer);
        _simulator.Characters.Add(seller);

        await _simulator.FinishDay();

        var OfferToSell = (from offer in station.localOffers
                           where offer.Offerer == buyer
                           select offer).FirstOrDefault();
        var OfferToBuy = (from offer in station.localOffers
                           where offer.Offerer == seller
                          select offer).FirstOrDefault();


        var speculator = new Character()
        {
            Name = "speculator I",
            Behavior = new SpeculatorBehavior()
            {
                OfferToSell = OfferToSell,
                OfferToBuy = OfferToBuy
            },
            Place = station,
            moneyBalance = 500
        };
        _simulator.Characters.Add(speculator);
        Console.WriteLine($"OfferToBuy {OfferToBuy.pricePerOne}, OfferToSell {OfferToSell.pricePerOne}");

        //Act
        for (int i = 0; i < 10; i++)
        {
            await _simulator.FinishDay();
            Console.WriteLine($"          OfferToBuy {OfferToBuy.pricePerOne}, OfferToSell {OfferToSell.pricePerOne}");
        }
        //Assert

        Console.WriteLine($"        cargos");
        foreach ( var theCargo in station.cargos)
        {
            Console.WriteLine($"{theCargo.Owner.Name}, {theCargo.Quantity}");
        }
        Console.WriteLine($"        offers");
        foreach (var theOffer in station.localOffers)
        {
            Console.WriteLine($"{theOffer.Offerer.Name}, {theOffer.pricePerOne}");
        }

        Assert.That(station.cargos.Count, Is.EqualTo(1));
        Assert.That(station.cargos.FirstOrDefault().Owner, Is.EqualTo(buyer));
        //Assert.That(station.localOffers.Count, Is.EqualTo(1));
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
            Place = station,
            moneyBalance = 500
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

    [Test]
    public async Task Do_ChoosingBestDeal()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        for (int i = 0; i < 2; i++)
        {
            var character = new Character()
            {
                Name = $"Linda {i+1}",
                Behavior = new StupidBuyerBehavior()
                { TypeToBuy = ItemType.food, StartPrice = (i+1)*100 },
                Place = station,
                moneyBalance = 500*(i+1)
            };

            _simulator.Characters.Add(character);
        }


        var seller = new Character()
        {
            Name = "Fred",
            Behavior = new StupidSellerBehavior(),
            Place = station
        };

        _simulator.Characters.Add(seller);

        var Cargo = new Item()
        {
            Owner = seller,
            Type = ItemType.food,
            Quantity = 500

        };
        station.cargos.Add(Cargo);

        var speculator = new Character()
        {
            Name = "German",
            Behavior = new SpeculatorBehavior(),
            Place = station,
            moneyBalance = 50000
        };

        _simulator.Characters.Add(speculator);

        //Act
       await _simulator.SkipDays(10);

        Console.WriteLine($"{station.CargoView()}  \n {station.OfferView()}");
    }

    [Test]
    public async Task Do_SoldUselessItems()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var buyer = new Character()
        {
            Name = "buyer I",
            Behavior = new StupidBuyerBehavior()
            { TypeToBuy = ItemType.fuel },
            Place = station,
            moneyBalance = 500
        };

        _simulator.Characters.Add(buyer);

        var speculator = new Character()
        {
            Name = "speculator I",
            Behavior = new SpeculatorBehavior(),
            Place = station,
            moneyBalance = 500
        };
        _simulator.Characters.Add(speculator);
        var cargo = new Item()
        {
            Owner = speculator,
            Type = ItemType.fuel,
            Quantity = 1
        };
        cargo.TransitToNewLocation(null, station);
        //Act
        await _simulator.SkipDays(3);

        //Assert
        Console.WriteLine($"        cargos");
        foreach (var theCargo in station.cargos)
        {
            Console.WriteLine($"{theCargo.Owner.Name}, {theCargo.Quantity}");
        }
        Console.WriteLine($"        offers");
        foreach (var theOffer in station.localOffers)
        {
            Console.WriteLine($"{theOffer.Offerer.Name}, {theOffer.pricePerOne}");
        }

        Assert.That(station.cargos.Count, Is.EqualTo(1));
        Assert.That(station.cargos.FirstOrDefault().Owner, Is.EqualTo(buyer));
    }
}
