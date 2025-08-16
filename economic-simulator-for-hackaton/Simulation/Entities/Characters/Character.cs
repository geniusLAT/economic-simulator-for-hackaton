using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters;

public class Character : Actor
{
    public Location? Place { get; set; }

    public IBehavior Behavior { get; set; }

    public string Name { get; set; }

    public void Do()
    {
        
    }
}
