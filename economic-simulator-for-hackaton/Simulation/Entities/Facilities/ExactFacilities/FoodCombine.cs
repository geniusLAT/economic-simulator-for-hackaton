using Simulation.Entities.Facilities.FacilityBehavior;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities.Facilities;

public class FoodCombine : ProducingFacility, IScaleableFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

    public virtual ItemType ScaleUpItem => ItemType.farmingEquipment;

    public uint Level { get; set; } = 1;

    public override List<ProductionRecipe> Recipes { get; set; } = [
        new()
        {
            ItemType = ItemType.food,
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

        return station.IsSunny;
    }
}
