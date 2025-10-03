using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters.BehaviorModel;

public class SpeculatorBehavior : IBehavior
{
    public Offer? OfferToSell;

    public Offer? OfferToBuy;

    public void Do(Character me)
    {

        Console.WriteLine("speculator is here");
        TrySellUseless(me);

        if (OfferToBuy is null || OfferToSell is null)
        {
            Console.WriteLine("null offers");
            StopCurrentTrading(me);
            return;
        }

       if (OfferToBuy.pricePerOne >= me.moneyBalance)
       {
            Console.WriteLine("money problem");
            StopCurrentTrading(me);
            return;
       }

        if (OfferToBuy.pricePerOne >= OfferToSell.pricePerOne)
        {
            Console.WriteLine("it will be bad deal");
            StopCurrentTrading(me);
            return;
        }

        if (OfferToBuy.ItemToSell is null)
        {
            Console.WriteLine("Seller has no product for me");
            StopCurrentTrading(me);
            return;
        }

        var cargoQuantity = OfferToSell.QuantityBorder;
        if (cargoQuantity > OfferToBuy.QuantityBorder)
        {
            cargoQuantity = OfferToBuy.QuantityBorder;
        }

        if (cargoQuantity is not null)
        {
            for (int i = 0; i < cargoQuantity; i++)
            {
                if (!Speculate(me))
                {
                    Console.WriteLine($"CQ, Speculating was over, new balance is {me.moneyBalance}");
                    return;
                }
            }
        }

        while (true)
        {
            if (!Speculate(me))
            {
                Console.WriteLine($"Speculating was over, new balance is {me.moneyBalance}");
                return;
            }
        }
    }

    public bool Speculate(Character me)
    {
        Console.WriteLine("Speculating");
        if (!OfferToBuy.accept(me, 1, (SpaceStation)me.Place))
        {
            Console.WriteLine($"Buing was failed");
            return false;
        }

        if(OfferToSell.accept(me, 1, (SpaceStation)me.Place))
        {
            return true;
        }
        Console.WriteLine($"{me.Name} bought but can not sell");
        return false;
    }

    public void StopCurrentTrading(Character me)
    {
        if (!FindNewPair(me))
        {
            Console.WriteLine($"Speculator {me.Name} has a " +
                $"problem, no deal to be done");
            //TODO stop speculating after some attempts
        }
    }

    public bool FindNewPair(Character me)
    {
        Console.WriteLine($"Speculator is looking for new pair");
        var station = me.Place as SpaceStation;
        if (station is null)
        {
            return false;
        }

        List<SpeculationPair> options = [];

        foreach (var offer in station.localOffers)
        {
            var type = offer.ItemType;
            var option = (from theOption  in options 
                            where theOption.ItemType == type
                          select theOption).FirstOrDefault();

            if (option is null)
            {
                Console.WriteLine($"Speculator found offer {offer} with new type ({offer.ItemType})");
                options.Add(
                    new()
                        {
                        OfferForSpeculatorToSell = !offer.IsOffererSelling ? offer : null,
                        OfferForSpeculatorToBuy = offer.IsOffererSelling ? offer : null
                    }
                    );
            }
            else
            {
                Console.WriteLine($"Speculator found offer {offer} with mentioned before type ({offer.ItemType})");
                if (offer.IsOffererSelling && option.OfferForSpeculatorToBuy is not null)
                {
                    //if speculator can buy cheaper he buy cheaper
                    if(option.OfferForSpeculatorToBuy.pricePerOne < offer.pricePerOne)
                    {
                        option.OfferForSpeculatorToBuy = offer;
                    }
                }
                if (offer.IsOffererSelling && option.OfferForSpeculatorToBuy is null)
                {
                    //first found offer to buy
                    option.OfferForSpeculatorToBuy = offer;
                }

                if (!offer.IsOffererSelling && option.OfferForSpeculatorToSell is not null)
                {
                    //if speculator can sell more expensive he sell more expensive
                    if (option.OfferForSpeculatorToSell.pricePerOne > offer.pricePerOne)
                    {
                        option.OfferForSpeculatorToSell = offer;
                    }
                }
                if (!offer.IsOffererSelling && option.OfferForSpeculatorToSell is null)
                {
                    //first found offer to sell
                    option.OfferForSpeculatorToSell = offer;
                }
            }
        }
        Console.WriteLine($"Speculator have to choose between {options.Count} options");
        var bestOption = options
        .Where(theOption => theOption.Contrast is not null)
        .Where(theOption => (theOption.OfferForSpeculatorToBuy!.QuantityBorder ?? uint.MaxValue ) > 0)
        .Where(theOption => (theOption.OfferForSpeculatorToSell!.QuantityBorder ?? uint.MaxValue) > 0)
        .Where(theOption => theOption.OfferForSpeculatorToBuy!.pricePerOne <= me.moneyBalance)       
        .Where(theOption => theOption.Contrast > 0)       
        .OrderByDescending(theOption => theOption.Contrast)
        .FirstOrDefault();

        //Console.WriteLine($"total: {options.Count}");
        //Console.WriteLine($"contrast: {options.Where(theOption => theOption.Contrast is not null).Count()}");
        //Console.WriteLine($"enough money: {options.Where(theOption => theOption.Contrast is not null)
        //.Where(theOption => theOption.OfferForSpeculatorToBuy!.pricePerOne <= me.moneyBalance).Count()}");

        if (bestOption is not null)
        {
            OfferToBuy = bestOption.OfferForSpeculatorToBuy;
            OfferToSell = bestOption.OfferForSpeculatorToSell;
            Console.WriteLine($"Speculator {me.Name} is going to speculate" +
                $" with {OfferToBuy!.ItemType}  between {OfferToBuy.Offerer.Name} " +
                $"and {OfferToSell!.Offerer.Name}, waiting " +
                $"for income near {bestOption.Contrast}");
            return true;
        };
        Console.WriteLine($"Speculator have no options");
        return false;
    }

    void TrySellUseless(Character me)
    {
        Console.WriteLine($"Speculator is looking for new pair");
        var station = me.Place as SpaceStation;
        if (station is null)
        {
            return;
        }
        List<Item> sellCandidates = me.Place.cargos
        .Where(cargo => cargo.Owner == me)
        .ToList();

         List<Offer> offers = station.localOffers
        .Where(offer => offer.Offerer != me)
        .Where(offer => !offer.IsOffererSelling)
        .ToList();

        foreach (var cargo in sellCandidates)
        {
            Console.WriteLine($"Speculator {me.Name} has useless {cargo.Type}");

            var bestPrice = 0f;
            Offer bestOffer = null;
            foreach (var offer in offers)
            {
                if( cargo.Type == offer.ItemType && offer.pricePerOne > bestPrice)
                {
                    bestOffer = offer;
                    bestPrice = offer.pricePerOne;
                }
            }

            if (bestOffer is not null)
            {
                int quantity =(int) Math.Min((long)cargo.Quantity, (long)(bestOffer.QuantityBorder ?? uint.MaxValue));
                Console.WriteLine($"Speculator {me.Name} sold useless" +
                    $" {quantity} items of {cargo.Type} for {bestPrice} per one");
                bestOffer.accept(me, quantity, station);
            }
        }
    }
}
