using Simulation.Entities.Characters;
using Simulation.Entities.Locations;

namespace Simulation.Simulators;

public class Simulator
{
    public List<Character> Characters { get; set; } = [];

    public List<PLayer> PLayerCharacters { get; set; } = [];

    public List<SpaceShip> spaceShips { get; set; } = [];

    public List<SpaceStation> spaceStations { get; set; } = [];

    public async Task GenerateWorldAsync()
    {

    }

    public async Task FinishDay()
    {
        foreach (var character in Characters)
        {
            character.Do();
        }
    }

    public async Task SkipDays(uint days)
    {
        for (var i = 0; i < days; i++)
        {
            await FinishDay();
        }
    }

    public SpaceStation? GetStationByCoord(uint x, uint y)
    {
         return spaceStations.FirstOrDefault(s => s.coordX == x && s.coordY == y);
    }
}
