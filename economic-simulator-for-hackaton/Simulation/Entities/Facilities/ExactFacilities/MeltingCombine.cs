using Simulation.Entities.Facilities.FacilityBehavior;
using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.Facilities;

public class MeltingCombine : ProducingFacility, IScaleableFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

    public uint Level { get; set; } = 1;

    public ItemType ScaleUpItem => ItemType.meltingEquipment;

    public override List<ProductionRecipe> Recipes { get; set; } = [
        new()
        {
            ItemType = ItemType.metal,
            requiredMaterials = [
                new(){
                    ItemType = ItemType.fuel,
                    Quantity = 1
                },
                 new(){
                    ItemType = ItemType.ore,
                    Quantity = 3
                }
                ]
        }
        ];

    public bool DoEnvironemtAllowToScaleUp()
    {
        return true;
    }
}
