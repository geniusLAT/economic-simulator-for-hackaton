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
        Behavior?.Do(this);
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

    public bool CloseOffer(Offer offer)
    {
        if (Place is SpaceStation)
        {
            var station = (SpaceStation)Place;
            station.localOffers.Remove(offer);
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

    public bool Unload(Item Cargo, uint quantityToUnload)
    {
        if(Cargo.Quantity < quantityToUnload)
        {
            return false;
        }

        if (Cargo.Quantity == quantityToUnload)
        {
            return Unload(Cargo);
        }

        if (Place is SpaceShip)
        {
            var ship = (SpaceShip)Place;
            if (ship.Parking is not null)
            {
                Cargo.Quantity -= quantityToUnload;

                var newCargo = new Item()
                {
                    Owner = Cargo.Owner,
                    Type = Cargo.Type,
                    Quantity = quantityToUnload
                };
                newCargo.TransitToNewLocation(null, ship.Parking);
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
                Cargo.TransitToNewLocation(ship, ship.Parking);
                return true;
            }
        }
        return false;
    }

    public bool Load(Item Cargo, SpaceShip ship, uint quantityToLoad)
    {
        if (Cargo.Quantity < quantityToLoad)
        {
            return false;
        }

        if (Cargo.Quantity == quantityToLoad)
        {
            return Load(Cargo, ship);
        }

        if (Place is SpaceStation)
        {
            if (ship.Parking == Place)
            {
                Cargo.Quantity -= quantityToLoad;

                var newCargo = new Item()
                {
                    Owner = Cargo.Owner,
                    Type = Cargo.Type,
                    Quantity = quantityToLoad
                };
                newCargo.TransitToNewLocation(null, ship);
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
                Cargo.TransitToNewLocation(station, ship);
                return true;
            }
        }
        return false;
    }
}
