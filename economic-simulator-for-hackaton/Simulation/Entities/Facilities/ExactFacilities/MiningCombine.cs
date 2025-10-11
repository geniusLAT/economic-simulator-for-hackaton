using Simulation.Entities.Facilities.FacilityBehavior;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities.Facilities;

public class MiningCombine : ProducingFacility, IScaleableFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

    public uint Level { get; set; } = 1;

    public override List<ProductionRecipe> Recipes { get; set; } = [
       new()
        {
            ItemType = ItemType.ore,
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
