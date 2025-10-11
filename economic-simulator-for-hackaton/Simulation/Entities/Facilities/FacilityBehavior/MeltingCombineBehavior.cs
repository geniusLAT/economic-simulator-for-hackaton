using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public sealed class MeltingCombineBehavior : CombineBehavior
{
    public MeltingCombineBehavior()
    {
        Materials = [];

        Materials.Add(new()
        {
            ItemType= ItemType.fuel,
            NeededPerProduction = 1
        });

        Materials.Add(new()
        {
            ItemType = ItemType.ore,
            NeededPerProduction = 3
        });
    }
}
