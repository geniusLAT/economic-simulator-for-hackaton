using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters.BehaviorModel;

public class WorkerBehavior : IBehavior
{
    public void Do(Character me)
    {
        if (me.Place is null)
        {
            return;
        }

        //Unloading all my goods to sell it
        if (me.Place is SpaceShip)
        {
            Console.WriteLine("Unloading");
            var ship = (SpaceShip)me.Place;
            if (ship.Parking is null)
            {
                //humbly waiting landing
                return;
            }

            foreach (var cargo in ship.cargos.ToArray())
            {
                if (cargo.Owner == me)
                {
                    me.Unload(cargo);
                }
            }
            me.Disembark();
        }

        var station = me.Place as SpaceStation;

        if (station is null)
        {
            Console.WriteLine($"worker {me.Name} is not at station");
            return;
        }

        if (!station.Workers.Contains(me))
        {
            Console.WriteLine($"worker {me.Name} is starting career");
            station.Workers.Add(me);
        }
    }
}
