using Simulation.Entities.Locations;
using static System.Collections.Specialized.BitVector32;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public abstract class CombineBehavior : IFacilityBehavior
{
    public virtual uint CountRawMaterialsForProducing(Facility facility)
    {
        return uint.MaxValue;
    }

    public virtual bool BuyRawMaterial(Facility facility)
    {
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

        var producingFacility = facility as ProducingFacility;
        if (producingFacility is null)
        {
            return;
        }
        var ItemToSell = (from cargo in station.cargos
                      where cargo.Owner == facility && cargo.Type == producingFacility.TypeOfProduct
                          select cargo).FirstOrDefault();

        var mustBeStored = (uint)3;
        var canProduce = (uint)1;
        var scaleableFacility = facility as IScaleableFacility;
        if (scaleableFacility is not null)
        {
            mustBeStored = 3 * scaleableFacility.Level;
            canProduce = scaleableFacility.Level;
        }
        canProduce = Math.Min(canProduce, CountRawMaterialsForProducing(facility));

        var wantToProduce = mustBeStored - (ItemToSell?.Quantity ?? 0);
        if (wantToProduce < 0) wantToProduce = 0;

        var producingToday = Math.Min(wantToProduce, canProduce);
        Console.WriteLine($"{facility.Name} can produce {canProduce}, " +
            $"wanna produce {wantToProduce}, " +
            $"will produce {producingToday}");
        producingFacility.Produce(producingToday);

        var productionSellingOffer = (from offer in station.localOffers
                                      where offer.Offerer == facility && offer.IsOffererSelling
                                      select offer
                                      ).FirstOrDefault();

        if (productionSellingOffer is null)
        {

            if (ItemToSell is null)
            {
                ItemToSell = (from cargo in station.cargos
                              where cargo.Owner == facility && cargo.Type == producingFacility.TypeOfProduct
                              select cargo).FirstOrDefault();
            }

            if (ItemToSell is null)
            {
                Console.WriteLine("Nothing to sell");
                return;
            }

            var price = 1f;
            if (facility.moneyBalance > 0)
            {
                price = facility.moneyBalance / mustBeStored;
            }
            var productionOffer = new Offer()
            {
                Offerer = producingFacility,
                IsOffererSelling = true,
                ItemToSell = ItemToSell,
                ItemType = producingFacility.TypeOfProduct,
                QuantityBorder = ItemToSell.Quantity,
                PriceBorder = 1,
                pricePerOne = price,
                HaveToMoveQuantityBorder = true
            };

            facility.Ceo.PublishOffer(productionOffer);
        }
        else
        {

            if (productionSellingOffer.ItemToSell is null)
            {
                productionSellingOffer.ItemToSell = (from cargo in station.cargos
                                                     where cargo.Owner == facility && cargo.Type == producingFacility.TypeOfProduct
                                                     select cargo).FirstOrDefault();    
            }
            productionSellingOffer.QuantityBorder = productionSellingOffer.ItemToSell?.Quantity ?? 0;
        }
        BuyRawMaterial(facility);    
        ScaleUpCheck(facility);
      
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
