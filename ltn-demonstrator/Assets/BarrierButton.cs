using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class BarrierButton : MonoBehaviour
{
    public GameObject barrierPrefab;
    public TextMeshProUGUI instructionText;
    private bool SpawnBarrier = false;
    private bool deleteMode = false; // Add this line

    Transform barrierParent;
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public BarrierManager barrierManager; // Add this line

    public void SaveGame()
    {
        if (barrierManager != null)
        {
            List<Barrier> barriers = new List<Barrier>();
            foreach (GameObject gameObject in barrierManager.allBarriers)
            {
                Barrier barrier = gameObject.GetComponent<Barrier>();
                if (barrier != null)
                {
                    barriers.Add(barrier);
                }
            }
            SaveSystem.SaveBarriers(barriers);
            Debug.Log("Game Saved");
        }
        else
        {
            Debug.LogError("No BarrierManager found in the scene.");
        }
    }

    public void DeleteABarrier()
    {
        instructionText.text = "Click on desired barrier to delete";
        deleteMode = true; // Add this line
    }

    public void DeleteSave()
    {
        foreach (GameObject barrierObject in barrierManager.allBarriers.ToArray())
        {
            // Remove from list
            barrierManager.allBarriers.Remove(barrierObject);

            // Destroy the barrier
            Destroy(barrierObject);
        }

        // Save the game to update the save file
        SaveGame();

        /*

        string filePath = Path.Combine(SAVE_FOLDER, "save.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Save Deleted");

            // Clear the barrier data list
            BarriersContainer data = new BarriersContainer { barriers = new List<BarrierData>() };

            // Save the empty list to the file
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);

            // Reload the barriers
            if (barrierManager != null)
            {
                barrierManager.LoadBarriers();
                Debug.Log("Barriers reloaded");
            }
            else
            {
                Debug.LogError("No BarrierManager assigned to the BarrierButton.");
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
        */
    }

    public void OnClick()
    {
        instructionText.text = "Click on desired barrier location";
        SpawnBarrier = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if (SpawnBarrier && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 worldPosition = hit.point;
                GameObject barrierObject = Instantiate(barrierPrefab, worldPosition, Quaternion.identity);
                Barrier barrier = barrierObject.GetComponent<Barrier>();
                if (barrier != null)
                {
                    Debug.Log("Barrier created at " + worldPosition);
                    if (barrierManager != null)
                    {
                        Debug.Log("Barrier List size: " + barrierManager.allBarriers.Count);
                        barrierManager.allBarriers.Add(barrierObject); // Add the GameObject, not the Barrier
                        SpawnBarrier = false;
                    }
                    else
                    {
                        Debug.LogError("No BarrierManager found in the scene.");
                    }
                }
                else
                {
                    Debug.LogError("No Barrier component found on the instantiated object.");
                }
            }
        }

        // Add this block
        if (deleteMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Barrier hitBarrier = hit.transform.GetComponent<Barrier>();
                if (hitBarrier != null)
                {
                    // Remove from list
                    barrierManager.allBarriers.Remove(hit.transform.gameObject);

                    // Destroy the barrier
                    Destroy(hit.transform.gameObject);

                    // Save the game to update the save file
                    SaveGame();

                    // Exit delete mode
                    deleteMode = false;
                }
            }
        }
    }
}