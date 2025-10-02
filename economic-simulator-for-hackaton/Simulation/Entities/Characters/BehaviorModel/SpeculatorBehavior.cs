using Simulation.Entities.Locations;

namespace Simulation.Entities.Characters.BehaviorModel;

public class SpeculatorBehavior : IBehavior
{
    public Offer? OfferToSell;

    public Offer? OfferToBuy;

    public void Do(Character me)
    {
        Console.WriteLine("speculator is here");

        if (OfferToBuy is null || OfferToSell is null)
        {
            Console.WriteLine("null offers");
            StopDicision();
            return;
        }

       if (OfferToBuy.pricePerOne >= me.moneyBalance)
       {
            Console.WriteLine("money problem");
            StopDicision();
            return;
       }

        if (OfferToBuy.pricePerOne >= OfferToSell.pricePerOne)
        {
            Console.WriteLine("it will be bad deal");
            StopDicision();
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

        return OfferToSell.accept(me,1, (SpaceStation)me.Place);

    }

    public void StopDicision()
    {

    }
}
