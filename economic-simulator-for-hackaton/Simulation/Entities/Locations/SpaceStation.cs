using Simulation.Entities.Facilities;
using Simulation.Utilities;

namespace Simulation.Entities.Locations;

public class SpaceStation : Location
{
    public List<SpaceShip> parkedShips { get; set; } = [];

    public List<Offer> localOffers { get; set; } = [];

    public List<Facility> facilities { get; set; } = [];

    public uint MaxLevelOfMining { get; set; } = 0;

    public uint MaxLevelOfFuel { get; set; } = 0;

    public bool IsSunny { get; set; } = false;

    public override string View()
    {
        var result = $"Станция называется {Name}, находится по координатам {coordX}, {coordY}\n";
        result += $"Уровень пригодности для добычи руды: {MaxLevelOfMining}\n";
        result += $"Уровень пригодности для добычи топлива: {MaxLevelOfFuel}\n";
        var SunnyString = "Нет";
        if (IsSunny)
        {
            SunnyString = "Да";
        }
        result += $"Пригодность для синтеза еды: {SunnyString}\n";
        result += OfferView();
        result += ShipsView();

        return result;
    }

    public string OfferView()
    {
        var result = $"Торговых предложений: {localOffers.Count}\n";
        if (localOffers.Count < 1)
        {
            return result ;
        }

        var drawer = new TableDrawer();
        drawer.AddLine(new List<string>() {
            "Номер",
        "Наименование товара",
        "вид предложения",
        "цена за штуку",
        "верхний предел товара",
        "Автор предложения"
        });

        for (int i = 0; i < localOffers.Count; i++)
        {
            Offer? offer = localOffers[i];
            drawer.AddLine(offer.ToStringList(i+1));
        }
        return result + drawer.Draw(true);
    }

    public string ShipsView()
    {
        var result = $"Кораблей припарковано: {parkedShips.Count}\n";
        if (parkedShips.Count < 1)
        {
            return result;
        }

        var drawer = new TableDrawer();
        drawer.AddLine(new List<string>() {
            "Номер",
        "Название судна",
        "Владелец",
        "Капитан"
        });

        for (int i = 0; i < parkedShips.Count; i++)
        {
            drawer.AddLine(parkedShips[i].ToStringList(i+1));
        }
        return result + drawer.Draw(true);
    }

    public string FacilitiesView()
    {
        var result = $"Предприятий основано: {facilities.Count}\n";
        if (facilities.Count < 1)
        {
            return result;
        }

        var drawer = new TableDrawer();
        drawer.AddLine(new List<string>() {
            "Номер",
        "Название ",
        "Владелец",
        "Директор",
        "Род деятельности"
        });

        for (int i = 0; i < facilities.Count; i++)
        {
            drawer.AddLine(facilities[i].ToStringList(i + 1));
        }
        return result + drawer.Draw(true);
    }

    public void SpeculatorSpawnCheck()
    {
        var offersToSell = (from offer in localOffers
                            where offer.IsOffererSelling
                            select offer 
                            );
        Offer firstOffer = null;
        Offer secondOffer = null;
        float contrast = 0;

        foreach ( var offer in offersToSell )
        {
            var bestOffer = (from potentialOffer in localOffers
                             where !potentialOffer.IsOffererSelling
                             orderby potentialOffer.pricePerOne
                             select potentialOffer
                             ).LastOrDefault();
            if (bestOffer == null)
            {
                continue;
            }
            var localContrast = bestOffer.pricePerOne - offer.pricePerOne;
            if (contrast < localContrast)
            {
                contrast = localContrast;
                firstOffer = offer;
                secondOffer = offer;
            }
        }
    }
}
