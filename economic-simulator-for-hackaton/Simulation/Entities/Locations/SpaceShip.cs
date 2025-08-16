using Simulation.Entities.Characters;
using Simulation.Entities.Items;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace Simulation.Entities.Locations;

public class SpaceShip : Location
{
    public Actor? Owner { get; set; }

    public Character? Captain { get; set; }

    public SpaceStation? Parking {  get; set; }

    public override string View()
    {
        var result = $"Корабль называется {Name}, находится по координатам {coordX}, {coordY}\n";
        result += $"Владелец: {Owner?.Name ?? "-"}\n";
        result += $"Капитан: {Captain?.Name ?? "-"}\n";
        if(Parking is not null)
        {
            result += $"Посажен на станции {Parking.Name}\n";
        }
        else
        {
            result += $"Находится в открытом космосе\n";
        }

        return result;
    }

    public bool Land(SpaceStation station)
    {
        coordX = station.coordX;
        coordY = station.coordY;
        Parking = station;

        station.parkedShips.Add(this);

        return true;
    }

    public bool TakeOff()
    {
        if (Parking is null) 
        { 
            return false;
        }
        Parking.parkedShips.Remove(this);
        Parking = null;

        return true;
    }

    public List<string> ToStringList(int index)
    {
        ///////$"Наименование товара \t вид предложения \t цена за штуку \t верхний предел товара \t Автор предложения"; 
        return new List<string>()
        {
            index.ToString(),
            Name,
            Owner.Name,
            Captain.Name,
        };
    }
}
