using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public sealed class MachineryCombineBehavior : CombineBehavior
{
    public MachineryCombineBehavior()
    {
        Materials = [];

        Materials.Add(new()
        {
            ItemType= ItemType.fuel,
            NeededPerProduction = 1
        });

        Materials.Add(new()
        {
            ItemType = ItemType.metal,
            NeededPerProduction = 10
        });
    }
}
