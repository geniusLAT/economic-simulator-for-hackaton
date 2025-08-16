using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Entities.Items;

public class Item
{
    public Actor? Owner { get; set; }

    public required ItemType Type { get; set; }
}
