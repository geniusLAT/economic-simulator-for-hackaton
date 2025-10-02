using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities.Facilities;

public class FuelCombine : ProducingFacility, IScaleableFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

    public override ItemType TypeOfProduct => ItemType.fuel;

    public virtual ItemType ScaleUpItem => ItemType.fuelProducingEquipment;

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

    public bool DoEnvironemtAllowToScaleUp()
    {
        var station = Place as SpaceStation;
        if (station is null)
        {
            return false;
        }

        uint currentLevelOfFuel = 0;
        foreach (var facility in station.facilities)
        {
            var fuelFacility = facility as FuelCombine;
            if (fuelFacility is null) continue;

            currentLevelOfFuel += fuelFacility.Level;
        }

        return station.MaxLevelOfFuel > currentLevelOfFuel;
    }
}
