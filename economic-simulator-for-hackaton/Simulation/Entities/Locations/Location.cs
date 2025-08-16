using Simulation.Entities.Items;

namespace Simulation.Entities.Locations;

public abstract class Location
{
    public uint coordX { get; set; }

    public uint coordY { get; set; }

    public List<Item> cargos { get; set; } = [];

    public abstract string View();
}
