using System.Collections.Generic;
using UnityEngine;

public enum BarrierType
{
    BlockAll,
    BlockAllMotorVehicles,
    BlockHeavyTraffic,
    BusOnly,
    BusAndTaxiOnly,
}

public static class BarrierTypeProperties
{
    private static readonly Dictionary<BarrierType, List<ModeOfTransport>> barrierRestrictions;

    static BarrierTypeProperties()
    {
        barrierRestrictions = new Dictionary<BarrierType, List<ModeOfTransport>>
        {
            { BarrierType.BlockAll, new List<ModeOfTransport> { ModeOfTransport.Bicycle, ModeOfTransport.Car, ModeOfTransport.SUV, ModeOfTransport.Van, ModeOfTransport.Taxi, ModeOfTransport.Bus } },
            { BarrierType.BlockAllMotorVehicles, new List<ModeOfTransport> { ModeOfTransport.Car, ModeOfTransport.SUV, ModeOfTransport.Van, ModeOfTransport.Taxi, ModeOfTransport.Bus } },
            { BarrierType.BlockHeavyTraffic, new List<ModeOfTransport> { ModeOfTransport.SUV, ModeOfTransport.Van, ModeOfTransport.Bus } },
            { BarrierType.BusOnly, new List<ModeOfTransport> { ModeOfTransport.Bicycle, ModeOfTransport.Car, ModeOfTransport.SUV, ModeOfTransport.Van, ModeOfTransport.Taxi } },
            { BarrierType.BusAndTaxiOnly, new List<ModeOfTransport> { ModeOfTransport.Bicycle, ModeOfTransport.Car, ModeOfTransport.SUV, ModeOfTransport.Van } },
        };
    }

    public static List<ModeOfTransport> GetBlockedModes(BarrierType barrierType)
    {
        return barrierRestrictions.ContainsKey(barrierType) ? barrierRestrictions[barrierType] : new List<ModeOfTransport>();
    }
}