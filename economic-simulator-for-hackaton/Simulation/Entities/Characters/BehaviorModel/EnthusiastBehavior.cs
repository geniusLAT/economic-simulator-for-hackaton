using Simulation.Entities.Facilities;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Locations;

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
                MyFacility =  StarterUp.StartUp(me, type, me, cargo, "name");
                if (MyFacility is not null)
                {
                    return true;
                }
            }
        }
        return false;
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
        var myCargos = station.cargos.Where(cargo => cargo.Owner == me);
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

        me.Behavior = new CeoBehavior()
        { 
            myFacilities = [ facility ]
        };
    }
}
