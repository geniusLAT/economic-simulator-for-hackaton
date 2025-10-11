using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public class ProductionRecipe
{
    public required ItemType ItemType { get; init; }

    public Offer? OfferToBuyProduced { get; set; }

    public required List<MaterialRequired> requiredMaterials { get; init; } = [];
}
