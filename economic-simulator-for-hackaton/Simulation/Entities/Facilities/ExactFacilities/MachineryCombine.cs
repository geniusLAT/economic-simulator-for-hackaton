using Simulation.Entities.Facilities.FacilityBehavior;
using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.Facilities;

public class MachineryCombine : ProducingFacility
{
    public Offer? ScaleUpEquipmentBuyInOffer {  get; set; }

    public ItemType ScaleUpItem => ItemType.machineryProducingEquipment;

    public override List<ProductionRecipe> Recipes { get; set; } = [
        new()
        {
            ItemType = ItemType.miningEquipment,
            requiredMaterials = [
                new(){
                    ItemType = ItemType.fuel,
                    Quantity = 1
                },
                 new(){
                    ItemType = ItemType.metal,
                    Quantity = 10
                }
                ]
        },
        new()
        {
            ItemType = ItemType.fuelProducingEquipment,
            requiredMaterials = [
                new(){
                    ItemType = ItemType.fuel,
                    Quantity = 1
                },
                 new(){
                    ItemType = ItemType.metal,
                    Quantity = 10
                }
                ]
        },
        new()
        {
            ItemType = ItemType.farmingEquipment,
            requiredMaterials = [
                new(){
                    ItemType = ItemType.fuel,
                    Quantity = 1
                },
                 new(){
                    ItemType = ItemType.metal,
                    Quantity = 10
                }
                ]
        },
        new()
        {
            ItemType = ItemType.meltingEquipment,
            requiredMaterials = [
                new(){
                    ItemType = ItemType.fuel,
                    Quantity = 1
                },
                 new(){
                    ItemType = ItemType.metal,
                    Quantity = 10
                }
                ]
        },
        new()
        {
            ItemType = ItemType.machineryProducingEquipment,
            requiredMaterials = [
                new(){
                    ItemType = ItemType.fuel,
                    Quantity = 1
                },
                 new(){
                    ItemType = ItemType.metal,
                    Quantity = 10
                }
                ]
        }
        ];

    public bool DoEnvironemtAllowToScaleUp()
    {
        return true;
    }
}
