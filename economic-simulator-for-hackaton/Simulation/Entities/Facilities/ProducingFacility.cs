using Simulation.Entities.Items;

namespace Simulation.Entities.Facilities;

public abstract class ProducingFacility : Facility
{
    public virtual ItemType TypeOfProduct => ItemType.food;

    public uint consecutiveFullCapacityDays { get; set; }

    public uint ProducedToday { get; set; }

    public abstract bool Produce(uint quantity);

    public override void FinishDay()
    {
        if (this is IScaleableFacility)
        {
            if (((IScaleableFacility)this).Level == ProducedToday)
            {
                consecutiveFullCapacityDays++;
            }
            else
            {
                consecutiveFullCapacityDays = 0;
            }
        }
        else
        {
            if(ProducedToday>0)
            {
                consecutiveFullCapacityDays++;
            }
            else
            {
                consecutiveFullCapacityDays = 0;
            }
        }
            ProducedToday = 0;
    }
}
