using Simulation.Entities.Items;

namespace Simulation.Entities.Locations;

public abstract class Location
{
    public string Name { get; set; } = "Новая";

    public uint coordX { get; set; }

    public uint coordY { get; set; }

    public List<Item> cargos { get; set; } = [];

    public abstract string View();
}
