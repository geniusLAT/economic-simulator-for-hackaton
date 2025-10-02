using Simulation.Simulators;
using Simulation.Utilities;

namespace SimulationTests.Simulators.PlayerPromptProcessorTests;

public class DrawTests
{
    private Simulator _simulator;

    private PlayerPromptProcessor _playerPromptProcessor;

    [SetUp]
    public void Setup()
    {
       
    }

    [Test]
    public async Task Draw_drawnCorreclty()
    {
        //Append
        var drawer = new TableDrawer();
        //Act
        for (int i = 0; i < 4; i++)
        {
            List<string> lines = new List<string>();
            for (int j = 0; j < 5; j++)
            {
                lines.Add(new string('a', i+j));
            }
            drawer.AddLine(lines);
        }
        var result = drawer.Draw();
        //Assert

        var expected = @"|   |a   |aa   |aaa   |aaaa   |
|a  |aa  |aaa  |aaaa  |aaaaa  |
|aa |aaa |aaaa |aaaaa |aaaaaa |
|aaa|aaaa|aaaaa|aaaaaa|aaaaaaa|
";
        Console.WriteLine(result);
        Assert.That(result, Is.EqualTo(expected.Replace("\r","")));
    }

    [Test]
    public async Task Draw_drawnCorrecltyANotherWay()
    {
        //Append
        var drawer = new TableDrawer();
        //Act
        for (int i = 0; i < 4; i++)
        {
            List<string> lines = new List<string>();
            for (int j = 0; j < 5; j++)
            {
                lines.Add(new string('a', 5 - i + j));
            }
            drawer.AddLine(lines);
        }
        var result = drawer.Draw();
        //Assert

        var expected = @"|aaaaa|aaaaaa|aaaaaaa|aaaaaaaa|aaaaaaaaa|
|aaaa |aaaaa |aaaaaa |aaaaaaa |aaaaaaaa |
|aaa  |aaaa  |aaaaa  |aaaaaa  |aaaaaaa  |
|aa   |aaa   |aaaa   |aaaaa   |aaaaaa   |
";
        Console.WriteLine(result);
        Assert.That(result, Is.EqualTo(expected.Replace("\r", "")));
    }

}
