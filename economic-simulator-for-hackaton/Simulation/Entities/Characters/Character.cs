using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters;

public class Character : Actor
{
    public Location? Place { get; set; }

    public IBehavior Behavior { get; set; }

    public void Do()
    {
        
    }

    public bool Disembark()
    {
        if (Place is SpaceShip)
        {
            var ship = (SpaceShip)Place;
            if (ship.Parking is not null)
            {
                Place = ship.Parking;
                return true;
            }
        }
        return false;
    }
}
