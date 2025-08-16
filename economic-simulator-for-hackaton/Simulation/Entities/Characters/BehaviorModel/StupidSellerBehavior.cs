using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Entities.Characters.BehaviorModel;

public class StupidSellerBehavior : IBehavior
{
    List<Offer> myOffers;

    public void Do(Character me)
    {
        if(me.Place is null)
        {
            return;
        }

        //Unloading all my goods to sell it
        if(me.Place is SpaceShip)
        {
            var ship = (SpaceShip)me.Place;
            if(ship.Parking is null)
            {
                //humbly waiting landing
                return;
            }

            foreach(var cargo in ship.cargos)
            {
                if (cargo.Owner == me)
                {
                    me.Unload(cargo);
                }
            }
            me.Disembark();
        }

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
                pricePerOne = price
            };

            if (me.PublishOffer(offer))
            {
                myOffers.Add(offer);
            }
        }
    }
}
