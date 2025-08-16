using Simulation.Entities.Items;

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
        if (IsOffererSelling)
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
        if (IsOffererSelling)
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
            if(Offerer.moneyBalance < PriceBorder)
            {
                PriceBorder = Offerer.moneyBalance;
            }

            if (WasUsedYesterday == 0)
            {
                pricePerOne += pricePerOne * 0.1f;
                if (pricePerOne > PriceBorder)
                {
                    pricePerOne = PriceBorder;
                }
                WasUsedYesterday = 0;
                return pricePerOne;
            }
            else
            {
                pricePerOne -= pricePerOne * 0.1f;
                WasUsedYesterday = 0;
                return pricePerOne;
            }
        }
    }
}
