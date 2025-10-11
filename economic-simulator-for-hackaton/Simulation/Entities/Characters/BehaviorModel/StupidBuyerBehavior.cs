using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters.BehaviorModel;

public class StupidBuyerBehavior : IBehavior
{
    Offer? myOffer;

    public ItemType TypeToBuy;

    public float StartPrice = 1f;

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

        var offer = new Offer() 
        {
            Offerer = me,
            ItemType = TypeToBuy,
            IsOffererSelling = false,
            pricePerOne = StartPrice,
            PriceBorder = me.moneyBalance,
            QuantityBorder = null
        };

        if (me.PublishOffer(offer))
        {
            myOffer = offer;
        }
    }
}
