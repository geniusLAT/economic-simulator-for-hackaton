using Simulation.Entities.Characters;
using Simulation.Entities.Facilities;
using Simulation.Entities.Facilities.Facilities;
using Simulation.Entities.Items;
using Simulation.Entities.Locations;
using System.Diagnostics.Metrics;

namespace Simulation.Entities;

public static class StarterUp
{
    public  static readonly Dictionary<Type, ItemType> FacilityToEquipmentMap = new Dictionary<Type, ItemType>
    {
        { typeof(MiningCombine), ItemType.miningEquipment },
        { typeof(FuelCombine), ItemType.fuelProducingEquipment }, 
        { typeof(FoodCombine), ItemType.farmingEquipment }, 
        { typeof(MeltingCombine), ItemType.meltingEquipment }
    };

    public static ItemType? GetEquipmentType(Type facilityType)
    {
        if (FacilityToEquipmentMap.TryGetValue(facilityType, out ItemType equipmentType))
        {
            return equipmentType;
        }
        return null;
    }

    public static Type? GetFacilityType(ItemType equipmentType)
    {
        var pair = FacilityToEquipmentMap
            .FirstOrDefault(kvp => kvp.Value == equipmentType);
        return pair.Key;
    }

    public static Facility? StartUp(
        Character me, 
        Type facilityType, 
        Actor? Owner = null, 
        Item? item = null,
        string? name = null
        )
    {
        var expectedEquipmentType = GetEquipmentType(facilityType);
        if (expectedEquipmentType == null)
        {
            Console.WriteLine("No equipment found");
            return null;
        }
        var station = me.Place as SpaceStation;
        if (station is null)
        {
            Console.WriteLine("Facility can not be out of space station");
            return null;
        }

        if ( Owner is null)
        {
            Owner = me;
        }

        if (item is not null )
        {
            if (item.Type != expectedEquipmentType || item.Quantity < 1)
            {
                Console.WriteLine("This equipment can not be used for that start up");
                return null;
            }
        }
        else
        {
            item = me.Place.cargos.FirstOrDefault(
                cargo => (cargo.Owner == me || cargo.Owner == Owner)
                && cargo.Type == expectedEquipmentType
                && cargo.Quantity > 0
                );
            if (item is null)
            {
                Console.WriteLine("No equipment found");
                return null;
            }
        }

        if (!CheckEnvironment(station, facilityType))
        {
            Console.WriteLine("Station does not allow to start uo such a facility");
            return null;
        }

        Facility newFacility = (Facility)Activator.CreateInstance(facilityType);

        if (newFacility is null)
        {
            Console.WriteLine("Failed to start up");
            return null;
        }

        newFacility.Owner = Owner;
        newFacility.Place = station;

        newFacility.Name = name ?? GenerateFacilityName(newFacility);

        item.Quantity --;
        if (item.Quantity == 0)
        {
            me.Place.cargos.Remove(item);
        }

        return newFacility;
    }

    public static bool CheckEnvironment(SpaceStation station, Type type)
    {
        if (type == typeof(FoodCombine))
        {
            return station.IsSunny;
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

            return station.MaxLevelOfFuel > currentLevelOfFuel;
        }
        if (type == typeof(MiningCombine))
        {
            uint currentLevelOfMining = 0;
            foreach (var facility in station.facilities)
            {
                var miningFacility = facility as MiningCombine;
                if (miningFacility is null) continue;

                currentLevelOfMining += miningFacility.Level;
            }

            return station.MaxLevelOfMining > currentLevelOfMining;
        }

        return true;
    }

    public static string GenerateFacilityName(Facility facility)
    {
        return "some facility";
    }
}
