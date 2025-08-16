using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Entities.Items;

public class Item
{
    public Actor? Owner { get; set; }

    public uint Quantity { get; set; } = 1;

    public required ItemType Type { get; set; }

    public static string TypeToString(ItemType type)
    {
        switch (type)
        {
            case ItemType.ore:
                return "Руда";
            case ItemType.metal:
                return "Металл";
            case ItemType.fuel:
                return "Топливо";
            case ItemType.food:
                return "Продовольствие";
            default:
                return "Нечто";
        }
    }

}
