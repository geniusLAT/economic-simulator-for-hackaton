using Simulation.Entities.Characters;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.PlayerPromptProcessorTests;

public class ProcessUnloadCommandTests
{
    private Simulator _simulator;

    private PlayerPromptProcessor _playerPromptProcessor;

    [SetUp]
    public void Setup()
    {
        _simulator = new();
        _playerPromptProcessor = new(_simulator);
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedSucessfully()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Груз разгружен";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedSucessfullyWithAccurateQuantity()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1 10", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Груз разгружен";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.cargos, Is.Empty);
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedSucessfullyWithAccurateQuantityNotAllCargo()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1 5", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Груз разгружен";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.cargos[0].Quantity, Is.EqualTo(5));
        Assert.That(station.cargos[0].Quantity, Is.EqualTo(5));
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedNegativeNumber()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1 -5", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Количество груза должно быть положительным";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedAbracadabraNotNumber()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1 Abracadabra", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Abracadabra - некорректное количество груза";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedButToldNoIndex()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Вы не указали номер груза для выгрузки";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedNowhere()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = null;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Вы находитесь нигде, тут нет груза";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedAtTheStation()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = station;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Чтобы разгружать корабль надо подняться на его борт. Вы находитесь на станции. У вас есть возможность загружать";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task ProcessUnloadCommand_UnloadedMoreThenExitsts()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
            Name = "Zeus II"
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        var ship = new SpaceShip()
        {
            coordX = 0,
            coordY = 0,
            Name = "Pegasus",
            Captain = pLayer,
            Owner = pLayer,
            Parking = station
        };
        _simulator.spaceShips.Add(ship);
        var cargo = new Item()
        {
            Owner = pLayer,
            Quantity = 10,
            Type = ItemType.ore
        };
        cargo.TransitToNewLocation(null, ship);

        pLayer.Place = ship;

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("разгрузить 1 50", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = "Количество груза на корабле равно 10, нельзя выгрузить  50 единиц, это больше чем имеется на борту";
        Assert.That(result, Is.EqualTo(expected));
        Assert.That(ship.cargos[0].Quantity, Is.EqualTo(10));
    }
}
