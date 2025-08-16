using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters;

public class Character : Actor
{
    public Location? Place { get; set; }

    public IBehavior Behavior { get; set; }

    public void Do()
    {

    }

    public bool PublishOffer(Offer offer)
    {
        if (Place is SpaceStation)
        {
            var station = (SpaceStation)Place;
            station.localOffers.Add(offer);
            return true;
        }
        return false;
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

    public bool Embark(SpaceShip ship)
    {
        if (Place is SpaceStation)
        {
            var station = (SpaceStation)Place;
            if (ship.Parking == station)
            {
                Place = ship;
                return true;
            }
        }
        return false;
    }

    public bool Unload(Item Cargo)
    {
        if (Place is SpaceShip)
        {
            var ship = (SpaceShip)Place;
            if(ship.Parking is not null)
            {
                ship.cargos.Remove(Cargo);
                ship.Parking.cargos.Add(Cargo);
                return true;
            }
        }
        return false;
    }

    public bool Load(Item Cargo, SpaceShip ship)
    {
        if (Place is SpaceStation)
        {
            var station = (SpaceStation)Place;
            if (ship.Parking == station)
            {
                station.cargos.Remove(Cargo);
                ship.cargos.Add(Cargo);
                return true;
            }
        }
        return false;
    }
}
