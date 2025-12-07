using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities;

public class JobOffer
{
    public required Actor Offerer { get; set; }

    public float Salary { get; set; }

    public float SalaryBorder { get; set; }

    public uint WorkersNeeded { get; set; } = 1;

    public bool Frozen { get; set; }

    public bool Done { get; set; }

    public void Accept()
    {
        Done = true;
    }
}
