using Simulation.Entities.Items;
using Simulation.Utilities;

namespace Simulation.Entities.Locations;

public abstract class Location
{
    public string Name { get; set; } = "Новая";

    public uint coordX { get; set; }

    public uint coordY { get; set; }

    public List<Item> cargos { get; set; } = [];

    public abstract string View();

    public string CargoView()
    {
        var result = "Грузов: ";
        if (cargos.Count < 1)
        {
            return result + "0";
        }

        var drawer = new TableDrawer();
        drawer.AddLine(new List<string>() {
            "Номер",
        "Наименование товара",
        "Количество",
        "Владелец"
        });

        uint sumQuantity = 0;
        for (int i = 0; i < cargos.Count; i++)
        {
            sumQuantity += cargos[i].Quantity;
            drawer.AddLine(cargos[i].ToStringList(i + 1));
        }
        return $"{result}{sumQuantity}\n{drawer.Draw(true)}";
    }
}
