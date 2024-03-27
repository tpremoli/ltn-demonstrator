using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TravellerManager : MonoBehaviour
{
    [System.Serializable]
    public class VehiclePrefabTypePair
    {
        public GameObject vehiclePrefab;
        public VehicleType vehicleType;
    }

    public static TravellerManager Instance;

    // pick random model and material
    [SerializeField]
    public List<VehiclePrefabTypePair> vehiclePrefabTypePairs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject pickRandomModelAndMaterial(VehicleType type)
    {
        var filteredList = vehiclePrefabTypePairs.Where(pair => pair.vehicleType == type).ToList();
        if (filteredList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, filteredList.Count);
            return filteredList[randomIndex].vehiclePrefab;
        }

        return null; // Or handle this case as needed
    }
    public GameObject GetManagerObject(){
        return this.gameObject;
    }
}
