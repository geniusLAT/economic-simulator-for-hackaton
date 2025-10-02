using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters.BehaviorModel;

public class StupidSellerBehavior : IBehavior
{
    List<Offer> myOffers = [];

    public void Do(Character me)
    {
        Console.WriteLine("Stupid Seller here");

        if (me.Place is null)
        {
            return;
        }

        //Unloading all my goods to sell it
        if(me.Place is SpaceShip)
        {
            Console.WriteLine("Unloading");
            var ship = (SpaceShip)me.Place;
            if(ship.Parking is null)
            {
                //humbly waiting landing
                return;
            }

            foreach(var cargo in ship.cargos.ToArray())
            {
                if (cargo.Owner == me)
                {
                    me.Unload(cargo);
                }
            }
            me.Disembark();
        }

        if(me.Place.cargos is null) return;
        if(me.Place.cargos.Count == 0) return;

        List<Item> newOfferCandidates = me.Place.cargos
        .Where(cargo => cargo.Owner == me && !myOffers.Any(offer => offer.ItemToSell == cargo))
        .ToList();


        foreach (var newOfferCandidate in newOfferCandidates)
        {
            var price = 1f;
            if (me.moneyBalance > 0)
            {
                price = me.moneyBalance / newOfferCandidate.Quantity;
            }
            var offer = new Offer() 
            {
                Offerer = me,
                ItemToSell = newOfferCandidate,
                ItemType = newOfferCandidate.Type,
                IsOffererSelling = true,
                //In base NPC will try to double his balance
                pricePerOne = price,
                PriceBorder = 0
            };

            if (me.PublishOffer(offer))
            {
                myOffers.Add(offer);
            }
        }

        foreach (var offer in myOffers.ToArray())
        {
            if(offer.ItemToSell is null || offer.ItemToSell.Owner != me)
            {
                Console.WriteLine($"{me.Name} stopped selling {offer.ItemType}, sold out");
                offer.ItemToSell = null;
                offer.QuantityBorder = 0;
                myOffers.Remove(offer);
                me.CloseOffer(offer);
            }
        }
    }
}
