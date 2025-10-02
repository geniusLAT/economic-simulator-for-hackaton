using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities.Facilities;

public class MiningCombine : ProducingFacility, IScaleableFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

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

    public bool DoEnvironemtAllowToScaleUp()
    {
        var station = Place as SpaceStation;
        if (station is null)
        {
            return false;
        }

        uint currentLevelOfMining = 0;
        foreach (var facility in station.facilities)
        {
            var miningFacility = facility as MiningCombine;
            if (miningFacility is null) continue;

            currentLevelOfMining += miningFacility.Level;
        }

        return station.MaxLevelOfMining > currentLevelOfMining;
    }
}
