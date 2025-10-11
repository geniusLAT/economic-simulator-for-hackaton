using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using static System.Collections.Specialized.BitVector32;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public abstract class CombineBehavior : IFacilityBehavior
{
    public List<StoredRawMaterial>? Materials { get; set; }

    public virtual uint CountRawMaterialsForProducing(ProducingFacility facility, ProductionRecipe recipe)
    {
        Console.WriteLine("CountRawMaterialsForProducing");

        if (Materials is null)
        {
            return 0;
        }

        uint count = uint.MaxValue;

        var requiredStoredMaterials = Materials
            .Where(material => recipe.requiredMaterials
            .Any(r => r.ItemType == material.ItemType));

        foreach (StoredRawMaterial material in requiredStoredMaterials)
        {

            //Looks unaccurate, TODO make it dictionary
            var recipeReq = recipe.requiredMaterials
                .Where(m => m.ItemType == material.ItemType)
                .FirstOrDefault()!;

            var storedMaterial = facility.Place.cargos
            .Where(cargo => cargo.Owner == facility)
            .Where(cargo => cargo.Type == material.ItemType)
            .FirstOrDefault();
            if (storedMaterial is null)
            {
                Console.WriteLine($"{facility.Name} has no {material.ItemType}, no production can be done");
                return 0;
            }

            var materialEnoughFor = storedMaterial.Quantity / recipeReq.Quantity;
            if (count > materialEnoughFor)
            {
                count = (uint)materialEnoughFor;
            }
            Console.WriteLine($"{facility.Name} has enough {material.ItemType} for {materialEnoughFor} productions");
        }

        Console.WriteLine($"{facility.Name} has enough materials for {count} productions");
        return count;
    }

    public virtual List<StoredRawMaterial> FillMaterialsList(Facility facility)
    {
        var prodFacility = facility as ProducingFacility;

        if (prodFacility is null)
        {
            return [];
        }
        var result = new List<StoredRawMaterial>();

        foreach (var recipe in prodFacility.Recipes)
        {
            foreach( var neededMaterial in recipe.requiredMaterials)
            {
                var storedThat = result
                    .Where(stored => stored.ItemType == neededMaterial.ItemType)
                    .FirstOrDefault();

                if( storedThat is null)
                {
                    result.Add(new() 
                    {
                        ItemType = neededMaterial.ItemType,
                        NeededPerProduction = neededMaterial.Quantity
                    }
                    );
                }
            }
        }

        return result;
    }

    public virtual bool BuyRawMaterial(Facility facility)
    {
        if (Materials is null)
        {
            Materials = FillMaterialsList(facility);
        }

        foreach (StoredRawMaterial material in Materials)
        {
            var materialOffer = facility.myOffers
            .Where(offer => offer.ItemType == material.ItemType)
            .FirstOrDefault();

            if (materialOffer is null)
            {
                materialOffer = new()
                {
                    Offerer = facility,
                    ItemType = material.ItemType,
                    IsOffererSelling = false,
                    pricePerOne = material.CostPrice,
                    PriceBorder = facility.moneyBalance,
                    HaveToMoveQuantityBorder = true,
                    QuantityBorder = material.NeededPerProduction
                };
                Console.WriteLine($"{facility.Name} buys {material.ItemType}");
                facility.Ceo.PublishOffer(materialOffer);
                facility.myOffers.Add(materialOffer);
            }
            else
            {
                var materialStock = facility.Place.cargos
               .Where(cargo => cargo.Type == material.ItemType)
               .Where(cargo => cargo.Owner == facility)
               .FirstOrDefault();

                if (materialStock is not null && materialStock.Quantity > 0)
                {
                    var boughtFromLastCheck = materialStock.Quantity - material.StoredUnits;
                    if (boughtFromLastCheck < 0) boughtFromLastCheck = 0;
                    var boughtTodayStock = materialOffer.WasUsedYesterday;
                    var boughtPreviousDay = boughtFromLastCheck - boughtTodayStock;
                    var sum = material.StoredUnits * material.CostPrice
                        + boughtTodayStock * materialOffer.pricePerOne
                        + boughtPreviousDay * material.RememberedMarketPrice;

                    Console.WriteLine($"sum = {material.StoredUnits}(material.StoredUnits) * " +
                        $"{material.CostPrice}(material.CostPrice)\n +" +
                        $"{boughtTodayStock}(boughtTodayStock) * " +
                        $"{materialOffer.pricePerOne}(materialOffer.pricePerOne)\n + " +
                        $"{boughtPreviousDay}(boughtPreviousDay) * " +
                        $"{material.RememberedMarketPrice}(material.RememberedMarketPrice)");



                    material.CostPrice = sum / materialStock.Quantity;
                    Console.WriteLine($"sum = {sum}, quantity = {materialStock.Quantity}, {material.ItemType} costPrice is {material.CostPrice}");

                    material.RememberedMarketPrice = materialOffer.pricePerOne;
                    material.StoredUnits = materialStock.Quantity;
                    materialOffer.PriceBorder = facility.moneyBalance;

                    //buys per day not more than can spend that day and next
                    var okToBuy=  2 * material.NeededPerProduction;
                    var scalableFacility = facility as IScaleableFacility;
                    if (scalableFacility != null)
                    {
                        okToBuy = 2 * scalableFacility.Level * material.NeededPerProduction;
                    }
                    if(materialStock.Quantity > 5 * okToBuy)
                    {
                        okToBuy = 0;
                    }

                    materialOffer.QuantityBorder = okToBuy;
                }
            }
        }
        return true;
    }

    public void Do(Facility facility)
    {
        var station = facility.Place as SpaceStation;
        if (station is null)
        {
            return;
        }

        if (facility.Ceo is null)
        {
            Console.WriteLine("Nobody to operate");
            return;
        }

        if (facility.Ceo.Place != facility.Place)
        {
            Console.WriteLine("CEO is far away");
            return;
        }
        BuyRawMaterial(facility);
        ProcessProduction(facility, station);
        ScaleUpCheck(facility);
    }

    public virtual void FindOutWhatCanBeProducedToday(ProducingFacility facility, SpaceStation station)
    {
        foreach (var recipe in facility.Recipes)
        {
            var product = recipe.OfferToBuyProduced?.ItemToSell;

            if (product is null)
            {
                product = (from cargo in station.cargos
                               where cargo.Owner == facility && cargo.Type == recipe.ItemType
                               select cargo).FirstOrDefault();

                if (recipe.OfferToBuyProduced is not null)
                {
                    recipe.OfferToBuyProduced.ItemToSell = product;
                }
            }
            
            var canProduce = (uint)1;
            var scaleableFacility = facility as IScaleableFacility;
            if (scaleableFacility is not null)
            {
                recipe.MustBeStored = 3 * scaleableFacility.Level;
                canProduce = scaleableFacility.Level;
            }
            canProduce = Math.Min(canProduce, CountRawMaterialsForProducing(facility, recipe));

            var wantToProduce = recipe.MustBeStored - (product?.Quantity ?? 0);
            if (wantToProduce < 0) wantToProduce = 0;

            var producingToday = Math.Min(wantToProduce, canProduce);
            recipe.canProduceToday = producingToday;

            Console.WriteLine($"{facility.Name} can produce {canProduce}, " +
            $"wanna produce {wantToProduce}, " +
            $"may produce {producingToday}");
        }
    }

    public void RecalculateAllProfit(ProducingFacility facility)
    {
        foreach (var recipe in facility.Recipes)
        {
            recipe.CalculateProfit();
        }
    }

    //TODO rewrite it. It should check what we can produce, 
    // then find out what is the best thing to produce
    //then produce it and put selling offer
    public virtual void ProcessProduction(Facility facility, SpaceStation station)
    {
        var producingFacility = facility as ProducingFacility;
        if (producingFacility is null)
        {
            return;
        }
        FindOutWhatCanBeProducedToday(producingFacility, station);
        RecalculateAllProfit(producingFacility);
        producingFacility.Recipes.Sort();
        var chosenRecipe = producingFacility.Recipes.FirstOrDefault();
        if (chosenRecipe is null || chosenRecipe.canProduceToday < 1)
        {
            Console.WriteLine($"{facility.Name} chose not to produce at all");
            if (chosenRecipe is null)
            {
                Console.WriteLine($"No Recipe");
                return;
            }
            if(chosenRecipe.canProduceToday < 1)
            {
                Console.WriteLine($"can not produce {chosenRecipe.ItemType}");
            }
            return;
        }
        Console.WriteLine($"{facility.Name} chose to produce {chosenRecipe.ItemType}");
        ManageProduction(producingFacility, chosenRecipe);
        ManageSelling(producingFacility, station, chosenRecipe);
    }

    public void ManageSelling(ProducingFacility facility, SpaceStation station, ProductionRecipe recipe)
    {
        var productionSellingOffer = recipe.OfferToBuyProduced ?? (from offer in station.localOffers
                                      where offer.Offerer == facility 
                                      && offer.IsOffererSelling
                                      && offer.ItemType == recipe.ItemType
                                      select offer
                                      ).FirstOrDefault();

        if (productionSellingOffer is null)
        {
           
           var ItemToSell = (from cargo in station.cargos
                              where cargo.Owner == facility && cargo.Type == recipe.ItemType
                              select cargo).FirstOrDefault();

            if (ItemToSell is null)
            {
                Console.WriteLine("Nothing to sell");
                return;
            }

            var price = 1f;
            if (facility.moneyBalance > 0)
            {
                price = facility.moneyBalance / recipe.MustBeStored;
                Console.WriteLine($"start price {price}");
            }
            productionSellingOffer = new Offer()
            {
                Offerer = facility,
                IsOffererSelling = true,
                ItemToSell = ItemToSell,
                ItemType = ItemToSell.Type,
                QuantityBorder = ItemToSell.Quantity,
                PriceBorder = 1,
                pricePerOne = price,
                HaveToMoveQuantityBorder = true
            };
            recipe.OfferToBuyProduced = productionSellingOffer;
            facility.Ceo!.PublishOffer(productionSellingOffer);
        }
        else
        {

            if (productionSellingOffer.ItemToSell is null)
            {
                productionSellingOffer.ItemToSell = (from cargo in station.cargos
                                                     where cargo.Owner == facility && cargo.Type == recipe.ItemType
                                                     select cargo).FirstOrDefault();
            }
            productionSellingOffer.QuantityBorder = productionSellingOffer.ItemToSell?.Quantity ?? 0;
        }
        productionSellingOffer.PriceBorder = CalculateUnitCost(recipe);
    }

    public virtual float CalculateUnitCost(ProductionRecipe recipe)
    {
        var requiredStoredMaterials = Materials!
           .Where(material => recipe.requiredMaterials
           .Any(r => r.ItemType == material.ItemType));

        var UnitCost = 0f;
        foreach (var material in requiredStoredMaterials)
        {
            Console.WriteLine($"{material.ItemType} unit cost is {material.CostPrice * material.NeededPerProduction}");
            UnitCost += material.CostPrice * material.NeededPerProduction;
        }
        Console.WriteLine($"total unit cost is {UnitCost}");
        recipe.UnitCost = UnitCost;
        if (recipe.OfferToBuyProduced is not null)
        {
            recipe.OfferToBuyProduced.PriceBorder = UnitCost;
        }
        return UnitCost;
    }

    public virtual void ManageProduction(ProducingFacility producingFacility, ProductionRecipe recipe)
    {
        if (!producingFacility.Produce(recipe.canProduceToday, recipe.ItemType))
        {
            return;
        }
        foreach (var material in Materials!)
        {
            var spentMaterials = recipe.canProduceToday * material.NeededPerProduction;
            if (spentMaterials > material.StoredUnits)
            {
                material.StoredUnits = 0;
            }
            else
            {
                material.StoredUnits -= spentMaterials;
            }
        }
    }

    public virtual bool ScaleUpCheck(Facility facility)
    {
        var scaleableFacility = facility as IScaleableFacility;
        if (scaleableFacility is null)
        {
            return false;
        }
        var producingFacility = facility as ProducingFacility;
        if (producingFacility is null)
        {
            return false;
        }
        if (producingFacility.consecutiveFullCapacityDays < 6)
        {
            return false;
        }

        if (!scaleableFacility.DoEnvironemtAllowToScaleUp())
        {
            return false;
        }
        var station = facility.Place as SpaceStation;
        if (station is null)
        {
            return false;
        }

        Console.WriteLine($"{facility.Name} wants to scale up");

        var ItemToSell = (from cargo in station.cargos
                          where cargo.Owner == facility && cargo.Type == scaleableFacility.ScaleUpItem
                          select cargo).FirstOrDefault();
        if (ItemToSell is not null)
        {
            if(scaleableFacility.ScaleUpEquipmentBuyInOffer is not null)
            {
                scaleableFacility.ScaleUpEquipmentBuyInOffer.QuantityBorder = 0;
                facility.Ceo?.CloseOffer(scaleableFacility.ScaleUpEquipmentBuyInOffer);
                facility.myOffers.Remove(scaleableFacility.ScaleUpEquipmentBuyInOffer);
                Console.WriteLine($"Stopped buing {scaleableFacility.ScaleUpEquipmentBuyInOffer.ItemType} for scaling up");
            }
            return scaleableFacility.ScaleUp(facility);
        }

        scaleableFacility.ScaleUpEquipmentBuyInOffer = (from offer in station.localOffers
                                      where offer.Offerer == facility 
                                      && !offer.IsOffererSelling
                                      && offer.ItemType == scaleableFacility.ScaleUpItem
                               select offer
                                      ).FirstOrDefault();
        if (scaleableFacility.ScaleUpEquipmentBuyInOffer is not null) 
        {
            scaleableFacility.ScaleUpEquipmentBuyInOffer.PriceBorder = facility.moneyBalance / 5;
            return false;
        }

        var priceBorder = 1f;
        if (facility.moneyBalance > 0)
        {
            priceBorder = facility.moneyBalance / 5;
        }
        scaleableFacility.ScaleUpEquipmentBuyInOffer = new Offer()
        {
            Offerer = facility,
            IsOffererSelling = false,
            ItemToSell = ItemToSell,
            ItemType = scaleableFacility.ScaleUpItem,
            QuantityBorder = 1,
            PriceBorder = priceBorder,
            pricePerOne = 1,
            HaveToMoveQuantityBorder = true
        };

        facility.Ceo.PublishOffer(scaleableFacility.ScaleUpEquipmentBuyInOffer);

        return true;
    }
}
