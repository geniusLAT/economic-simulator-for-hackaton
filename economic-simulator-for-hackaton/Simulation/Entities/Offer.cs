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

    public uint? Border { get; set; } = 1;

    public override string ToString()
    {
        var typeStr = "продаёт";
        if (IsOffererSelling)
        {
            typeStr = "скупает";
        }
        var BorderStr = "-";
        if (Border is not null)
        {
            BorderStr = Border.ToString();
        }
        ///////$"Наименование товара \t вид предложения \t цена за штуку \t верхний предел товара \t Автор предложения"; 
        return $"\t{Item.TypeToString(ItemType)}|\t {typeStr}|\t {pricePerOne}|\t { BorderStr}|\t {Offerer.Name}";
    }
}
