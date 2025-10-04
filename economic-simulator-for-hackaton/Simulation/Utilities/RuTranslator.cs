using Simulation.Entities.Facilities.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Utilities;

public static class RuTranslator
{

    public static string GetName(Type type)
    {
        if (type == typeof(MiningCombine))
            return "Шахта";
        if (type == typeof(MeltingCombine))
            return "Рудоплавильный комбинат";
        if (type == typeof(FoodCombine))
            return "Продуктовый комбинат";
        if (type == typeof(FuelCombine))
            return "Топливный комбинат";

        //Default facility
        if (type == typeof(FuelCombine))
            return "Предприятие";

        return "Нечто";
    }
}
