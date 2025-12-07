using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using static System.Collections.Specialized.BitVector32;

namespace Simulation.Entities.Facilities.FacilityBehavior;

public sealed class StupidRecruitingBehavior : IFacilityBehavior
{
    public void Do(Facility facility)
    {
        if (facility is not StupidRecruiter recriter)
        {
            return;
        }

        if (recriter.Place is not SpaceStation station)
        {
            return;
        }

        if (recriter.JobOffer is null)
        {
            recriter.JobOffer = new()
            {
                Offerer = recriter,
                Salary = 1,
                SalaryBorder = recriter.moneyBalance / 5,
                WorkersNeeded = 5
            };
            station.JobOffers.Add(recriter.JobOffer);
        }
        else
        {
            recriter.JobOffer.SalaryBorder = recriter.moneyBalance / 5;
            recriter.JobOffer.UpdatePrice();
        }
    }
}
