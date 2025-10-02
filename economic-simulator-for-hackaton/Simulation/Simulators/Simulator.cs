using Simulation.Entities.Characters;
using Simulation.Entities.Locations;

namespace Simulation.Simulators;

public class Simulator
{
    public List<Character> Characters { get; set; } = [];

    public List<PLayer> PLayerCharacters { get; set; } = [];

    public List<SpaceShip> spaceShips { get; set; } = [];

    public List<SpaceStation> spaceStations { get; set; } = [];

    public bool SkipDayWhenAllPlayersAreReady { get; set; } = false;

    public uint SecondsInCycle { get; set; } = 60;

    public async Task GenerateWorldAsync()
    {

    }

    public async Task FinishDay()
    {
        Console.WriteLine("Finishing day");

        foreach (var character in Characters)
        {
            Console.WriteLine($" character {character.Name} is finishing day");
            character.Do();
        }
        foreach (var station in spaceStations)
        {
            foreach (var offer in station.localOffers)
            {
               var newPrice = offer.UpdatePrice();
                Console.WriteLine($"new price is {newPrice}");
            }
            foreach (var facility in station.facilities)
            {
               facility.FinishDay();
            }
        }

        foreach (var spaceShip in spaceShips)
        {
            Console.WriteLine($" spaceShip {spaceShip.Name} is finishing day");
            spaceShip.FinishDay();
        }
        PLayerCharacters.ForEach(p => p.ReadyForDayFinishing = false);
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

    public async Task Simulate(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (SkipDayWhenAllPlayersAreReady) 
            {
                for (int i = 0; i < SecondsInCycle; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                    if (PLayerCharacters.All(p => p.ReadyForDayFinishing))
                    {
                        break;
                    }
                }
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(60), token);
            }
            await FinishDay();
        }
    }
}
