using Simulation.Entities.Locations;

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
            case ItemType.miningEquipment:
                return "Шахтёрское оборудование";
            case ItemType.fuelProducingEquipment:
                return "Оборудование для синтеза топлива";
            case ItemType.farmingEquipment:
                return "Фермерское оборудование";
            case ItemType.meltingEquipment:
                return "Плавильное оборудование";
            case ItemType.machineryProducingEquipment:
                return "Машиностроительное оборудование";
            default:
                return "Нечто";
        }
    }

    public void TransitToNewLocation(Location? oldLocation, Location location)
    {
        if (oldLocation is not null)
        {
            oldLocation.cargos.Remove(this);
        }

        var identicalCargo = location.cargos
        .FirstOrDefault(c => c.Owner == Owner && c.Type == Type);

        if (identicalCargo != null)
        {
            identicalCargo.Quantity += this.Quantity;
        }
        else
        {
            location.cargos.Add(this);
        }
    }

    public List<string> ToStringList(int index)
    {
        return new List<string>()
        {
            index.ToString(),
            Item.TypeToString(Type),
            Quantity.ToString(),
            Owner?.Name ?? "-",

        };
    }
}
