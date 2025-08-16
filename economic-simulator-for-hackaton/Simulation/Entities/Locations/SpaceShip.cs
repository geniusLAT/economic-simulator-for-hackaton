using Simulation.Entities.Characters;
using System.Xml.Linq;

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
}
