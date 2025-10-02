using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities.Facilities;

public class MeltingCombine : ProducingFacility, IScaleableFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

    public override ItemType TypeOfProduct => ItemType.metal;

    public uint Level { get; set; } = 1;

    public virtual ItemType ScaleUpItem => ItemType.meltingEquipment;

    public const uint FuelPerProduce = 1;

    public const uint OrePerProduce = 1;

    public override bool Produce(uint quantity)
    {
        if (quantity + ProducedToday > Level)
        {
            Console.WriteLine($"{Name} can not produce {quantity}");
            return false;
        }

        var oreNeeded = OrePerProduce * quantity;
        var ores = Place.cargos
            .Where(cargo => cargo.Owner == this)
            .Where(cargo => cargo.Type == ItemType.ore)
            .FirstOrDefault();

        if (ores is null || ores.Quantity < oreNeeded)
        {
            return false;
        }

        var fuelNeeded = FuelPerProduce * quantity;
        var fuel = Place.cargos
            .Where(cargo => cargo.Owner == this)
            .Where(cargo => cargo.Type == ItemType.fuel)
            .FirstOrDefault();

        if (fuel is null || fuel.Quantity < fuelNeeded)
        {
            return false;
        }

        ores.Quantity -=oreNeeded;
        if (ores.Quantity == 0)
        {
            Place.cargos.Remove(ores);
        }

        fuel.Quantity -= fuelNeeded;
        if (fuel.Quantity == 0)
        {
            Place.cargos.Remove(fuel);
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

    public bool DoEnvironemtAllowToScaleUp()
    {
        return true;
    }
}
