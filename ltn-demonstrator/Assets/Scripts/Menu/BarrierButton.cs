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

    BarrierManager barrierManager;

    void Start()
    {
        barrierManager = BarrierManager.Instance;
        if (barrierManager == null)
        {
            Debug.LogError("No BarrierManager found assigned to the BarrierButton.");
        }
    }

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

        Debug.Log("Deleted Barriers");

        // Save the game to update the save file
        SaveGame();

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
                Debug.Log("Barrier created at " + worldPosition);
                if (barrierManager != null)
                {
                    Debug.Log("Barrier List size: " + barrierManager.allBarriers.Count);
                    barrierManager.AddBarrier(worldPosition);
                    SpawnBarrier = false;
                }
                else
                {
                    Debug.LogError("No BarrierManager found in the scene.");
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