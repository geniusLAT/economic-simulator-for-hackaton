using Simulation.Entities.Facilities;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Facilities.FacilityBehavior;

namespace Simulation.Entities.Characters.BehaviorModel;

public class CeoBehavior : IBehavior
{
    public List<Facility> myFacilities = [];

    public void Do(Character me)
    {
        Console.WriteLine("CEO here");

        if (me.Place is null)
        {
            return;
        }

        foreach (Facility facility in myFacilities)
        {
            if (facility.Behavior is null)
            {
                if (!ChooseBehaviorForFacility(facility)) 
                {
                    continue;
                }
                Console.WriteLine($"{me.Name} finds strategy for {facility.Name}");
            }
            facility.Behavior!.Do(facility);
        }
    }

    public bool ChooseBehaviorForFacility(Facility facility)
    {
        switch (facility)
        {
            case MiningCombine combine:
                combine.Behavior = new MiningCombineBehavior();
                return true;
            case FuelCombine combine:
                combine.Behavior = new FuelCombineBehavior();
                return true;
            case FoodCombine combine:
                combine.Behavior = new FoodCombineBehavior();
                return true;
            case MeltingCombine combine:
                combine.Behavior = new MeltingCombineBehavior();
                return true;
            default:
                return false;
        }
    }
}
