using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public class ProductionRecipe : IComparable<ProductionRecipe>
{
    public required ItemType ItemType { get; init; }

    public Offer? OfferToBuyProduced { get; set; }

    public required List<MaterialRequired> requiredMaterials { get; init; } = [];

    public uint canProduceToday {  get; set; }

    public float UnitCost { get; set; } = 1;

    public float Profit { get; set; } = 0;

    public uint mustBeStored { get; set; } = 3;

    public void CalculateProfit()
    {
        Profit = -UnitCost;

        if (OfferToBuyProduced is not null)
        {
            Profit += OfferToBuyProduced.pricePerOne;
        }
    }

    public int CompareTo(ProductionRecipe? other)
    {
        if (other is null) return 1;

        if (OfferToBuyProduced is null && other.OfferToBuyProduced is null)
        {
            //if nothing was sold before sell one that can be made in bigger amount
            return other.canProduceToday.CompareTo(canProduceToday);
        }
        
        //prefer one that was not sold before. Diversity
        if (other.OfferToBuyProduced is null)
        {
            return 1;
        }
        if (OfferToBuyProduced is null)
        {
            return -1;
        }
        //prefer more profit
        return other.Profit
            .CompareTo(Profit);
    }
}
