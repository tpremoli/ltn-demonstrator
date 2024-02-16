using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class EditScreenMenu : MonoBehaviour
{
    
    public GameObject barrierPrefab;
    public TextMeshProUGUI instructionText;
    private bool SpawnBarrier = false;
    private bool deleteMode = false;

    Transform barrierParent;
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public BarrierManager barrierManager;
    private Graph graph;
    // Declare the CameraMovement variable
    public CameraMovement cameraMovement;

    void Start()
    {
        // Get the CameraMovement component
        cameraMovement = FindObjectOfType<CameraMovement>();

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

    public void OnDeleteBarrierPressed()
    {
        instructionText.text = "Click on desired barrier to delete";
        deleteMode = true;
    }

    public void OnDeleteSavePressed()
    {
        foreach (GameObject barrierObject in barrierManager.allBarriers.ToArray())
        {
            barrierManager.allBarriers.Remove(barrierObject);
            Destroy(barrierObject);
        }

        Debug.Log("Deleted Barriers");
        SaveGame();
    }

    public void OnAddBarrierPressed()
    {
        instructionText.text = "Click on desired barrier location";
        SpawnBarrier = true;
    }

    void Update()
    {
        if (SpawnBarrier)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Disable camera movement
                cameraMovement.canMove = false;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 worldPosition = hit.point;
                    Debug.Log("Barrier created at " + worldPosition);
                    if (barrierManager != null)
                    {
                        this.graph = GameObject.Find("Graph").GetComponent<Graph>();

                        Debug.Log("Barrier List size: " + barrierManager.allBarriers.Count);
                        barrierManager.AddBarrier(worldPosition);
                        SpawnBarrier = false;
                        instructionText.text = "To add a barrier, click on the button again. To delete a barrier, click on the delete button.";
                    }
                    else
                    {
                        Debug.LogError("No BarrierManager found in the scene.");
                    }
                }
            }
        }

        if (deleteMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Disable camera movement
                cameraMovement.canMove = false;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Barrier hitBarrier = hit.transform.GetComponent<Barrier>();
                    if (hitBarrier != null)
                    {
                        barrierManager.allBarriers.Remove(hit.transform.gameObject);
                        Destroy(hit.transform.gameObject);
                        SaveGame();
                        deleteMode = false;
                    }
                }
            }
        }

        // Re-enable camera movement when no mouse button is pressed
        if (!Input.GetMouseButton(0))
        {
            cameraMovement.canMove = true;
        }
    }
    

    public void OnPlayButtonPressed()
    {
        SaveGame();
        // Load the MenuProperMapScene scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("PlayMode");
    }
}