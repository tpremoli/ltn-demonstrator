using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class BarrierButton : MonoBehaviour
{
    public GameObject barrierPrefab;
    public TextMeshProUGUI instructionText;
    private bool SpawnBarrier = false;

    Transform barrierParent;
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public void SaveGame()
    {
        BarrierManager barrierManager = GameObject.FindObjectOfType<BarrierManager>();
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

    
    /*
    public void LoadGame()
    {
        BarrierManager barrierManager = GameObject.FindObjectOfType<BarrierManager>();
        if (barrierManager != null)
        {
            if (barrierParent == null)
            {
                barrierParent = Instantiate(new GameObject("BarrierParent")).transform;
            }

            for (int i = 0; i < barrierManager.allBarriers.Count; i++)
            {
                Vector3 position = new Vector3(barrierManager.allBarriers[i].transform.position.x, barrierManager.allBarriers[i].transform.position.y, barrierManager.allBarriers[i].transform.position.z);
                Quaternion rotation = Quaternion.Euler(barrierManager.allBarriers[i].transform.rotation.eulerAngles.x, barrierManager.allBarriers[i].transform.rotation.eulerAngles.y, barrierManager.allBarriers[i].transform.rotation.eulerAngles.z);
                GameObject barrierObject = Instantiate(barrierPrefab, position, rotation, barrierParent);
                Barrier barrier = barrierObject.GetComponent<Barrier>();
                barrierManager.allBarriers.Add(barrier);
            }
        }
        else
        {
            Debug.LogError("No BarrierManager found in the scene.");
        }
    }
    */

    public void DeleteSave()
    {
        string filePath = Path.Combine(SAVE_FOLDER, "save.json");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Save Deleted");
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
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
                    BarrierManager barrierManager = GameObject.FindObjectOfType<BarrierManager>();
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
    }
}