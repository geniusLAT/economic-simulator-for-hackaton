using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public class MaterialFound
{
    public required Item cargo { get; init; }

    public required uint Needed { get; init; }
}
