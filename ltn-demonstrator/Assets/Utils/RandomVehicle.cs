using UnityEngine;

public class RandomVehicle : MonoBehaviour
{
    public GameObject[] vehicleModels; // Array of vehicle models
    public Material[] vehicleMaterials; // Array of materials

    void Start()
    {
        // Randomly select a model and material
        GameObject selectedModel = vehicleModels[Random.Range(0, vehicleModels.Length)];
        Material selectedMaterial = vehicleMaterials[Random.Range(0, vehicleMaterials.Length)];

        // Instantiate the selected model
        GameObject instance = Instantiate(selectedModel, transform.position, transform.rotation);

        // Assign the selected material
        instance.GetComponent<MeshRenderer>().material = selectedMaterial;
    }
}
