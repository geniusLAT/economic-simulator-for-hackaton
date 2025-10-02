using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities;

public class Offer
{
    public required Actor Offerer { get; set; }

    public bool IsOffererSelling { get; set; } = false;

    public float pricePerOne { get; set; } = 1;

    public ItemType ItemType { get; set; }

    public Item? ItemToSell { get; set; }

    public uint WasUsedYesterday { get; set; }

    public uint? QuantityBorder { get; set; } = 1;

    //Will not sell cheaper, will not buy more expensive
    public float PriceBorder { get; set; }

    public override string ToString()
    {
        var typeStr = "продаёт";
        if (!IsOffererSelling)
        {
            typeStr = "скупает";
        }
        var BorderStr = "-";
        if (QuantityBorder is not null)
        {
            BorderStr = QuantityBorder.ToString();
        }
        ///////$"Наименование товара \t вид предложения \t цена за штуку \t верхний предел товара \t Автор предложения"; 
        return $"\t{Item.TypeToString(ItemType)}|\t {typeStr}|\t {pricePerOne}|\t { BorderStr}|\t {Offerer.Name}";
    }

    public List<string> ToStringList(int index)
    {
        var typeStr = "продаёт";
        if (!IsOffererSelling)
        {
            typeStr = "скупает";
        }
        var BorderStr = "-";
        if (QuantityBorder is not null)
        {
            BorderStr = QuantityBorder.ToString();
        }
        ///////$"Наименование товара \t вид предложения \t цена за штуку \t верхний предел товара \t Автор предложения"; 
        return new List<string>()
        {
            index.ToString(),
            Item.TypeToString(ItemType),
            typeStr,
            pricePerOne.ToString(),
            BorderStr,
            Offerer.Name,

        };
    }

    public float UpdatePrice()
    {
        Console.WriteLine($"    {Offerer.Name} updating price");
        if (IsOffererSelling)
        {
            if(WasUsedYesterday == 0)
            {
                pricePerOne -= pricePerOne * 0.1f;
                if (pricePerOne < PriceBorder)
                {
                    pricePerOne = PriceBorder;
                }
                WasUsedYesterday = 0;
                return pricePerOne;
            }
            else
            {
                //float salesRatio = (float)WasUsedYesterday / QuantityBorder;
                //if (salesRatio > 1f) salesRatio = 1f;
                //float maxIncrease = 0.15f;
                //float increaseRate = maxIncrease * salesRatio;

                var increaseRate = 0.1f;
                pricePerOne += pricePerOne * increaseRate;
                WasUsedYesterday = 0;
                return pricePerOne;
            }
        }
        else
        {
            if (Offerer.moneyBalance < PriceBorder)
            {
                PriceBorder = Offerer.moneyBalance;
            }

            if (WasUsedYesterday == 0)
            {
                
                pricePerOne += pricePerOne * 0.1f;
                if (pricePerOne == 0)
                {  
                    pricePerOne = 0.01f;
                }

                    if (pricePerOne > PriceBorder)
                {
                    Console.WriteLine($"price {pricePerOne} is on border {PriceBorder} ");
                    pricePerOne = PriceBorder;
                }
                WasUsedYesterday = 0;
                Console.WriteLine($"price is upper: {pricePerOne}");
                return pricePerOne;
            }
            else
            {
                pricePerOne -= pricePerOne * 0.1f;
                WasUsedYesterday = 0;
                Console.WriteLine($"price is lower: {pricePerOne}");
                return pricePerOne;
            }
        }
    }

    public bool accept(Actor accepter, int quantity, SpaceStation station)
    {
        var totalPrice = quantity * pricePerOne;

        if (quantity > QuantityBorder)
        {
            return false;
        }

        if (IsOffererSelling)
        {
            Console.WriteLine($"IsOffererSelling");
            if (accepter.moneyBalance < totalPrice)
            {
                Console.WriteLine($"not enough money 2");
                return false;
            }

            if (ItemToSell is null)
            {
                ItemToSell = (from cargo in station.cargos
                                    where cargo.Owner == Offerer && cargo.Type == ItemType
                                    select cargo).FirstOrDefault();
            }

            if (ItemToSell is null)
            {
                Console.WriteLine($"Nothing to sell, product is needed");
                return false; 
            }

            if (quantity > ItemToSell.Quantity)
            {
                Console.WriteLine($"Not enough product");
                return false;
            }

            if (quantity < ItemToSell.Quantity)
            {
                ItemToSell.Quantity -= (uint)quantity;

                var newItem = new Item()
                {
                   Type = ItemType,
                   Owner = accepter,
                   Quantity = (uint)quantity
                };
                newItem.TransitToNewLocation(null, station);
            }
            if (quantity == ItemToSell.Quantity)
            {
                ItemToSell.Owner = accepter;
                ItemToSell.TransitToNewLocation(station, station);
                ItemToSell = null;
            }

            WasUsedYesterday += (uint)quantity;
            accepter.moneyBalance -= totalPrice;
            Offerer.moneyBalance += totalPrice;
        }
        else
        {
            Item? itemToSell = (from cargo in station.cargos
                                where cargo.Owner == accepter && cargo.Type == ItemType
                                select cargo).FirstOrDefault();
            Console.WriteLine($"IsOfferer Buying");
            if (itemToSell is null)
            {
                Console.WriteLine($"Nothing to buy");
                return false;
            }

            if (Offerer.moneyBalance < totalPrice)
            {
                Console.WriteLine($"Not enough money to buy {Offerer.Name} " +
                    $"got only {Offerer.moneyBalance}, but has to pay {totalPrice}");
                return false;
            }

            if (itemToSell.Quantity < quantity)
            {
                Console.WriteLine($"not enough cargo to buy");
                return false;
            }
            if (quantity < itemToSell.Quantity)
            {
                itemToSell.Quantity -= (uint)quantity;

                var newItem = new Item()
                {
                    Type = ItemType,
                    Owner = Offerer,
                    Quantity = (uint)quantity
                };
                newItem.TransitToNewLocation(null, station);
            }
            if (quantity == itemToSell.Quantity)
            {
                itemToSell.Owner = Offerer;
                itemToSell.TransitToNewLocation(station, station);
            }
            WasUsedYesterday += (uint)quantity;
            accepter.moneyBalance += totalPrice;
            Offerer.moneyBalance -= totalPrice;

        }
        Console.WriteLine($"Deal is closed");
        return true;
    }
}
