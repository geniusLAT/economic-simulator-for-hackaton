using Simulation.Entities.Facilities.FacilityBehavior;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities.Facilities;

public class FuelCombine : ProducingFacility, IScaleableFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

    public virtual ItemType ScaleUpItem => ItemType.fuelProducingEquipment;

    public uint Level { get; set; } = 1;

    public override List<ProductionRecipe> Recipes { get; set; } = [
       new()
        {
            ItemType = ItemType.fuel,
            requiredMaterials = []
        }
       ];

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
