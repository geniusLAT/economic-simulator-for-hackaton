using Simulation.Entities.Characters.BehaviorModel;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;

namespace Simulation.Entities.Facilities;

public interface IScaleableFacility 
{
    public virtual Offer? ScaleUpEquipmentBuyInOffer { get { return null; } set { } }

    public virtual ItemType ScaleUpItem => ItemType.miningEquipment;

    public uint Level { get; set; }

    public virtual bool DoEnvironemtAllowToScaleUp()
    {
        return true;
    }

    public virtual bool ScaleUp()
    {
        Level++;
        return true;
    }
}
