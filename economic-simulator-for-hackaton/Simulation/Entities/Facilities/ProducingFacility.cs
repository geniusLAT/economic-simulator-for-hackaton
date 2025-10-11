using Simulation.Entities.Facilities.FacilityBehavior;
using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities;

public abstract class ProducingFacility : Facility
{
    public List<ProductionRecipe> Recipes { get; set; } = [];

    public virtual ItemType TypeOfProduct => ItemType.food;

    public uint consecutiveFullCapacityDays { get; set; }

    public uint ProducedToday { get; set; }

    public virtual bool Produce(uint quantity, ItemType itemType)
    {
        var recipe = Recipes.Where(recipe => recipe.ItemType == itemType).FirstOrDefault();
        if (recipe is null)
        {
            return false;
        }

        var found = new List<MaterialFound>();

        foreach (var material in recipe.requiredMaterials)
        {
            var materialNeeded = material.Quantity * quantity;
            var materialStores = Place.cargos
                .Where(cargo => cargo.Owner == this)
                .Where(cargo => cargo.Type == material.ItemType)
                .FirstOrDefault();

            if (materialStores is null || materialStores.Quantity < materialNeeded)
            {
                return false;
            }
            found.Add(new()
            {
                cargo = materialStores,
                Needed = materialNeeded
            });
        }

        foreach (var material in found)
        {
            material.cargo.Quantity -= material.Needed;
            if (material.cargo.Quantity == 0)
            {
                Place.cargos.Remove(material.cargo);
            }
        }

        var produced = new Item
        {
            Type = recipe.ItemType,
            Quantity = quantity,
            Owner = this
        };
        produced.TransitToNewLocation(null, Place);
        ProducedToday += quantity;
        return true;
    }

    public override void FinishDay()
    {
        if (this is IScaleableFacility)
        {
            if (((IScaleableFacility)this).Level == ProducedToday)
            {
                consecutiveFullCapacityDays++;
            }
            else
            {
                consecutiveFullCapacityDays = 0;
            }
        }
        else
        {
            if(ProducedToday>0)
            {
                consecutiveFullCapacityDays++;
            }
            else
            {
                consecutiveFullCapacityDays = 0;
            }
        }
            ProducedToday = 0;
    }
}
