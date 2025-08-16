using Simulation.Entities.Characters;
using Simulation.Entities.Locations;
using Simulation.Simulators;

namespace SimulationTests.Simulators.PlayerPromptProcessorTests;

public class ProcessPromptTests
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
    public async Task ProcessPrompt_PlayerDoesNotExist()
    {
        //Append
        
        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("помощь", Guid.NewGuid());

        //Assert
        Assert.That(result, Is.EqualTo("Вы мертвы или не существуете"));
    }

    [Test]
    public async Task ProcessPrompt_PlayerExistsButWrongCommand()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("ABCDEF", pLayer.Guid);

        //Assert
        Assert.That(result, Is.EqualTo("команда не распознана"));
    }

    [Test]
    public async Task ProcessPrompt_PlayerViewNowhere()
    {
        //Append
        var station = new SpaceStation()
        {
            coordX = 0,
            coordY = 0,
        };

        _simulator.spaceStations.Add(station);

        var pLayer = new PLayer()
        {
            Name = "Joe Doe",
            //Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр", pLayer.Guid);

        //Assert
        Assert.That(result, Is.EqualTo("Пустота и ничего более"));
    }

    [Test]
    public async Task ProcessPrompt_PlayerViewAtStation()
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
            Place = station
        };

        _simulator.Characters.Add(pLayer);
        _simulator.PLayerCharacters.Add(pLayer);

        //Act
        var result = await _playerPromptProcessor.ProcessPromptAsync("осмотр", pLayer.Guid);

        //Assert
        Console.WriteLine(result);

        var expected = @"Станция называется Zeus II, находится по координатам 0 0
Уровень пригодности для добычи руды: 0
Уровень пригодности для добычи топлива: 0
Пригодность для синтеза еды: Нет
";
        Assert.That(result, Is.EqualTo(expected.Replace("\r","")));
    }
}
