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

    public virtual bool ScaleUp(Facility facility)
    {
        var station = facility.Place as SpaceStation;
        if (station is null)
        {
            return false;
        }
        var ItemToSell = (from cargo in station.cargos
                          where cargo.Owner == facility && cargo.Type == ScaleUpItem
                          select cargo).FirstOrDefault();

        if (ItemToSell is null) 
        {
            return false;
        }

        ItemToSell.Quantity--;
        if (ItemToSell.Quantity < 1)
        {
            station.cargos.Remove(ItemToSell);
        }

        Level++;
        return true;
    }
}
