namespace Simulation.Entities.Locations;

public class SpaceStation : Location
{
    public List<SpaceShip> parkedShips { get; set; } = [];

    public uint MaxLevelOfMining { get; set; } = 0;

    public uint MaxLevelOfFuel { get; set; } = 0;

    public bool IsSunny { get; set; } = false;

    public override string View()
    {
        var result = $"Станция называется {Name}, находится по координатам {coordX}, {coordY}\n";
        result += $"Уровень пригодности для добычи руды: {MaxLevelOfMining}\n";
        result += $"Уровень пригодности для добычи топлива: {MaxLevelOfFuel}\n";
        var SunnyString = "Нет";
        if (IsSunny)
        {
            SunnyString = "Да";
        }
        result += $"Пригодность для синтеза еды: {SunnyString}\n";


        return result;
    }
}
