using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Entities.Characters.BehaviorModel;

public class StupidBuyerBehavior : IBehavior
{
    Offer? myOffer;

    public ItemType TypeToBuy;

    public void Do(Character me)
    {
        Console.WriteLine("Stupid buyer here");

        if (me.Place is null)
        {
            return;
        }

        if(me.Place is SpaceShip)
        {
            me.Disembark();
        }

        if (myOffer is not null)
        {
            myOffer.PriceBorder = me.moneyBalance;
            return;
        }

        var price = 1f;
        //if (me.moneyBalance > 0)
        //{
        //    price = me.moneyBalance / 2;
        //}
        var offer = new Offer() 
        {
            Offerer = me,
            ItemType = TypeToBuy,
            IsOffererSelling = false,
            pricePerOne = price,
            PriceBorder = me.moneyBalance
        };

        if (me.PublishOffer(offer))
        {
            myOffer = offer;
        }
    }
}
