using Simulation.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public class StoredRawMaterial
{
    public required ItemType ItemType { get; set; }

    public float CostPrice { get; set; } = 1;

    public uint StoredUnits { get; set; } = 0;

    public float RememberedMarketPrice { get; set; } = 1;

    public uint NeededPerProduction {  get; set; } = 1;
}
