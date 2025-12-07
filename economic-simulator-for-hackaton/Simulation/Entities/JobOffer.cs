using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities;

public class JobOffer
{
    public required Actor Offerer { get; set; }

    public float Salary { get; set; }
}
