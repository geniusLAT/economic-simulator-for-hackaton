using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public class MaterialRequired
{
    public required ItemType ItemType { get; init; }

    public required uint Quantity { get; init; }
}
