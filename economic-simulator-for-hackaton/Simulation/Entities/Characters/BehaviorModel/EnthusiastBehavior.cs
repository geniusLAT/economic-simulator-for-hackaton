using Simulation.Entities.Facilities;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Locations;
using Simulation.Utilities;

namespace Simulation.Entities.Characters.BehaviorModel;

public class EnthusiastBehavior : IBehavior
{
    public Facility? MyFacility = null;

    public void Do(Character me)
    {
        Console.WriteLine("Enthusiast here");

        var ship = me.Place as SpaceShip;
        if (ship is not null)
        {
            if (ship.Parking is null)
            {
                //humbly waiting landing
                return;
            }

            foreach (var cargo in ship.cargos.ToArray())
            {
                if (cargo.Owner == me)
                {
                    me.Unload(cargo);
                }
            }
            me.Disembark();
        }

        var station = me.Place as SpaceStation;
        if (station is null) 
        {
            return;
        }

        if (!TryStartUp(me, station))
        {
            if (BuySomethingForStartingUp(me, station))
            {
                TryStartUp(me, station);
            }
        }

        if (MyFacility is not null)
        {
            BecomeCeo(me, station, MyFacility);
        }
        //TODO think about buying ship
        //TODO think about buying facility
        //TODO think about food store

    }

    public bool TryStartUp(Character me, SpaceStation station)
    {
        var necessaryCargo = station.cargos.
            Where(cargo => 
            StarterUp.FacilityToEquipmentMap.ContainsValue(cargo.Type)
            && cargo.Owner == me
            ).ToList();

        if (!necessaryCargo.Any()) 
        {
            return false;
        }

        if (necessaryCargo.Any())
        {
            
            foreach (var cargo in necessaryCargo)
            {
                var type = StarterUp.GetFacilityType(cargo.Type);
                MyFacility =  StarterUp.StartUp(me, type, me, cargo, $"{me.Name}'s facility");
                if (MyFacility is not null)
                {
                    MyFacility.Name = ChooseFacilityName(MyFacility, me);
                    return true;
                }
            }
        }
        return false;
    }

    public string ChooseFacilityName(Facility facility, Character me)
    {
        var type = facility.GetType();
        var word = RuTranslator.GetName(type);

        var pseudoRandom = (long)(me.moneyBalance + me.Place?.coordX ?? 0);
        if (pseudoRandom < 0)
        {
            pseudoRandom = 0;
        }
        string candidateName = "";
        switch (pseudoRandom % 9)
        {
            case 0:
                candidateName = $"{word} <<{me.Place!.Name}>>";
                break;
            case 1:
                candidateName = NameVariator
                    .NameVariations[(int)pseudoRandom%NameVariator.NameVariations.Count]; 
                break;
            case 2:
                candidateName = $"{word} <<{NameVariator
                    .NameVariations[(int)pseudoRandom % NameVariator.NameVariations.Count]}>>";
                break;
            case 3:
                candidateName = NameVariator
                    .SovietWaveNameVariations[(int)pseudoRandom % NameVariator.SovietWaveNameVariations.Count];
                break;
            case 4:
                candidateName = $"{word} <<{NameVariator
                    .SovietWaveNameVariations[(int)pseudoRandom % NameVariator.SovietWaveNameVariations.Count]}>>";
                break;
            case 5:
                candidateName = $"{word} <<{NameVariator
                    .SovietWaveNameVariations[
                    (int)pseudoRandom % NameVariator.SovietWaveNameVariations.Count]}-{
                    (pseudoRandom % 85) + 10}>>";
                break;
            case 6:
                candidateName = $"{word} <<{NameVariator
                    .JapaneseNameVariations[
                    (int)pseudoRandom % NameVariator.JapaneseNameVariations.Count]}-{
                    (pseudoRandom % 85) + 10}>>";
                break;
            case 7:
                candidateName =  $"{word} <<{NameVariator
                    .JapaneseNameVariations[
                    (int)pseudoRandom % NameVariator.JapaneseNameVariations.Count]}>>";
                break;
            case 8:
                candidateName = NameVariator
                    .JapaneseNameVariations[
                    (int)pseudoRandom % NameVariator.JapaneseNameVariations.Count];
                break;
            default:
                break;
        }

        var count = (me.Place as SpaceStation).facilities.Count;
        if( count > 0)
        {
            candidateName += " " + RomanNumeralConverter.ToRoman(count);
        }

        return candidateName;
    }

    public bool BuySomethingForStartingUp(Character me, SpaceStation station)
    {
        var interestingOffers = station.localOffers.
            Where(offer =>
            StarterUp.FacilityToEquipmentMap.ContainsValue(offer.ItemType)
            && offer.IsOffererSelling
            && offer.pricePerOne < me.moneyBalance
            ).ToList();

        if (!interestingOffers.Any())
        { 
            return false;
        }
        Offer bestOffer = null;
        var bestInterestIndex = int.MinValue;
        foreach (var offer in interestingOffers)
        {
            var interestIndex = int.MinValue;

            var facilityType = StarterUp.GetFacilityType(offer.ItemType);
            if ( StarterUp.CheckEnvironment(station,facilityType))
            {
                interestIndex = 100;
                interestIndex += CountChallengerModifier(station, facilityType);
                interestIndex += CountGrowthPotentialModifier(station, facilityType);
            }

            if(interestIndex > bestInterestIndex 
                || (bestOffer is not null 
                && interestIndex == bestInterestIndex 
                && bestOffer.pricePerOne > offer.pricePerOne
                ))
            {
                bestInterestIndex = interestIndex;
                bestOffer = offer;
            }
        }
        if (bestOffer is not null)
        {
            return bestOffer.accept(me, 1, station);
        }
        return false;
    }

    public int CountChallengerModifier(SpaceStation station, Type type)
    {
        var modifier = 0;
        foreach (var challengingFacility in station.facilities)
        {
            if (challengingFacility.GetType() == type)
            {
                modifier--;
            }
        }

        return modifier;
    }

    public int CountGrowthPotentialModifier(SpaceStation station, Type type)
    {
        if(typeof(IScaleableFacility).IsAssignableFrom(type))
        {
            if (type == typeof(MiningCombine))
            {
                uint currentLevelOfMining = 0;
                foreach (var facility in station.facilities)
                {
                    var miningFacility = facility as MiningCombine;
                    if (miningFacility is null) continue;

                    currentLevelOfMining += miningFacility.Level;
                }
                var potential = (int)station.MaxLevelOfMining - (int)currentLevelOfMining;
                if (potential < 1)
                {
                    return -100;
                }
                return potential;
            }
            if (type == typeof(FuelCombine))
            {
                uint currentLevelOfFuel = 0;
                foreach (var facility in station.facilities)
                {
                    var fuelFacility = facility as FuelCombine;
                    if (fuelFacility is null) continue;

                    currentLevelOfFuel += fuelFacility.Level;
                }

                var potential = (int)station.MaxLevelOfFuel - (int)currentLevelOfFuel;
                if (potential < 1)
                {
                    return -100;
                }
                return potential;
            }

            return 10;
        }
        return 1;
    }

    public void BecomeCeo(Character me, SpaceStation station, Facility facility)
    {
        facility.moneyBalance = me.moneyBalance;
        me.moneyBalance = 0;
        var myCargos = station.cargos.Where(cargo => cargo.Owner == me && cargo.Quantity > 0);
        foreach (var cargo in myCargos)
        {
            cargo.Owner = facility;

            var price = 1f;
            if (facility.moneyBalance > 0)
            {
                price = facility.moneyBalance / cargo.Quantity;
            }
            var offer = new Offer()
            {
                Offerer = facility,
                ItemToSell = cargo,
                ItemType = cargo.Type,
                IsOffererSelling = true,
                //In base NPC will try to double his balance
                pricePerOne = price,
                PriceBorder = 0,
                QuantityBorder = cargo.Quantity,
                HaveToMoveQuantityBorder = true
            };

            Console.WriteLine($"{facility.Name} is selling useless {offer.ItemType}");

            if (me.PublishOffer(offer))
            {
                facility.myOffers.Add(offer);
            }
        }

        station.facilities.Add(facility);
        me.Behavior = new CeoBehavior()
        { 
            myFacilities = [ facility ]
        };
        facility.Ceo = me;
    }
}
