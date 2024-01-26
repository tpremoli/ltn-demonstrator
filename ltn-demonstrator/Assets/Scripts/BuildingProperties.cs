using System.Collections;
using System.Collections.Generic;

public static class BuildingProperties
{
    public static Dictionary<BuildingType, float> destinationWeights = new Dictionary<BuildingType, float>(){
            {BuildingType.Generic, 0.1f},
            {BuildingType.Residence, 1.0f},
            {BuildingType.Office, 2.0f},
            {BuildingType.Restaurant, 2.5f},
            {BuildingType.Shop, 4.0f},
            {BuildingType.ThroughTrafficDummy, 0.25f},
        };

    public static List<BuildingType> buildingTypes = new List<BuildingType>((BuildingType[])System.Enum.GetValues(typeof(BuildingType)));

    // Choose a random building type.
    public static BuildingType getRandomWeightedDestinationType()
    {
        float totalDestinationWeight = 0.0f;
        List<float> cumulativeWeights = new List<float>();

        // Get totals for all destination weights and populate cumulative weights list.
        foreach (KeyValuePair<BuildingType, float> destinationWeight in destinationWeights)
        {
            totalDestinationWeight += destinationWeight.Value;

            cumulativeWeights.Add(totalDestinationWeight);
        }

        // Select random float value.
        float r = UnityEngine.Random.value * totalDestinationWeight;

        // Iterate through cumulative weights to find index to select.
        int index = -1;
        for (int i = 0; i < cumulativeWeights.Count; i++)
        {
            float weight = cumulativeWeights[i];
            if (r <= weight)
            {
                index = i;
                break;
            }
        }

        return buildingTypes[index];
    }
}