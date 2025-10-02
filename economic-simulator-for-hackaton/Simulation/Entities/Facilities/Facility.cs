using Simulation.Entities.Characters;
using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities;

public abstract class Facility : Actor
{
    public Actor? Owner { get; set; }

    public Character? Ceo { get; set; }

    public required Location Place { get; set; }

    public IFacilityBehavior? Behavior { get; set; }

    public List<Offer> myOffers { get; set; } = [];

    public virtual void Do()
    {
        Behavior?.Do(this);
    }

    public virtual void FinishDay()
    {
        
    }
}
