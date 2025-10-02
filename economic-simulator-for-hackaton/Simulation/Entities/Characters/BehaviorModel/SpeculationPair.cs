using Simulation.Entities.Items;

namespace Simulation.Entities.Characters.BehaviorModel;

public class SpeculationPair
{
    public ItemType? ItemType { get; set; }

    private Offer? _offerForSpeculatorToBuy;

    public Offer? OfferForSpeculatorToBuy
    {
        get
        {
            return _offerForSpeculatorToBuy;
        }
        set
        {
            if(ItemType is null)
            {
                ItemType = value?.ItemType;
            }
            if(_offerForSpeculatorToSell is not null && value is not null)
            {
                Contrast = _offerForSpeculatorToSell.pricePerOne - value.pricePerOne;
            }
            _offerForSpeculatorToBuy = value;
        }
    }

    private Offer? _offerForSpeculatorToSell;

    public Offer? OfferForSpeculatorToSell
    {
        get
        {
            return _offerForSpeculatorToSell;
        }
        set
        {
            if (ItemType is null)
            {
                ItemType = value?.ItemType;
            }
            if (value is not null && _offerForSpeculatorToBuy is not null)
            {
                Contrast = value.pricePerOne - _offerForSpeculatorToBuy.pricePerOne;
            }
            _offerForSpeculatorToSell = value;
        }
    }
    public float? Contrast { get; set; }
}
