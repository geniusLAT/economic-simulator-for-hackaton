using Simulation.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Entities.Facilities.Facilities;

public class MiningCombine : ProducingFacility, IScaleableFacility
{
    public override ItemType TypeOfProduct => ItemType.ore;

    public uint Level { get; set; } = 1;

    public override bool Produce(uint quantity)
    {
        if (quantity + ProducedToday > Level)
        {
            Console.WriteLine($"{Name} can not produce {quantity}");
            return false;
        }

        var produced = new Item
        {
            Type = TypeOfProduct,
            Quantity = quantity,
            Owner = this
        };
        produced.TransitToNewLocation(null, Place);
        ProducedToday += quantity;
        return true;
    }
}
