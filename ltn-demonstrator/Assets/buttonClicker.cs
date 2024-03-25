using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;


public class ButtonClicker : MonoBehaviour
{
    private bool SpawnBarrier = false;

    private bool SpawnSensor = false;
    private bool deleteMode = false;

    Transform barrierParent;
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public BarrierManager barrierManager;
    public SensorManager sensorManager;
    private Graph graph;
    // Declare the CameraMovement variable
    public CameraMovement cameraMovement;
    [SerializeField] private UIDocument mainUIDocument;
    [SerializeField] private UIDocument addBarrierUIDocument;
    private VisualElement rootVisualElement;
    private Label instructionLabel;

    public BarrierType selectedBarrierType;
    private VisualElement addBarrierRoot;

    public CanvasRenderer uiElementRenderer;

public void HideUIElement()
{
    // This will hide the UI element without disabling the GameObject
    uiElementRenderer.SetAlpha(0);
}

public void ShowUIElement()
{
    // This will show the UI element
    uiElementRenderer.SetAlpha(1);
}

    private void OnEnable()
    {
        if (!mainUIDocument)
        {
            Debug.LogError("Main UIDocument is not assigned in the inspector.");
            return;
        }
        if (!addBarrierUIDocument)
        {
            Debug.LogError("Add Barrier UIDocument is not assigned in the inspector.");
            return;
        }

        mainUIDocument.gameObject.SetActive(true);
        addBarrierUIDocument.gameObject.SetActive(false);

        rootVisualElement = mainUIDocument.rootVisualElement;
        addBarrierRoot = addBarrierUIDocument.rootVisualElement;
        instructionLabel = rootVisualElement.Q<Label>("InstructionLabel");
        if (instructionLabel == null)
        {
            Debug.LogError("Instruction Label is not found in the Main UIDocument.");
            return;
        }

        SetupMainButtons();
    }


    private void SetupMainButtons()
    {
        // Set up the buttons for the main UI
        rootVisualElement.Q<Button>("PlayButton").clicked += OnPlayButtonPressed;
        rootVisualElement.Q<Button>("AddBarriersButton").clicked += OnAddBarrierMenuPressed;
        rootVisualElement.Q<Button>("DeleteBarrierButton").clicked += OnDeleteBarrierPressed;
        rootVisualElement.Q<Button>("DeleteSaveButton").clicked += OnDeleteSavePressed;
        rootVisualElement.Q<Button>("AddSensorsButton").clicked += OnAddSensorPressed;
    }

    private void SetupAddBarrierButtons()
    {
        addBarrierRoot = addBarrierUIDocument.rootVisualElement;

        Debug.LogError("Start");
        Debug.LogError(addBarrierRoot);
        // Set up the buttons for the add barrier UI
        addBarrierRoot.Q<Button>("BackButton").clicked += () => OnBackButtonPressed();
        Debug.LogError("Button1 Added");
        addBarrierRoot.Q<Button>("BlockAllButton").clicked += () => OnAddBlockAllBarrierPressed();
        Debug.LogError("Button2 Added");
        addBarrierRoot.Q<Button>("BlockMotorVehiclesButton").clicked += () => OnAddBlockAllMotorVehiclesBarrierPressed();
        Debug.LogError("Button3 Added");
        addBarrierRoot.Q<Button>("BlockHeavyButton").clicked += () => OnAddBlockAllMotorVehiclesBarrierPressed();
        Debug.LogError("Button4 Added");
        addBarrierRoot.Q<Button>("AllowBusButton").clicked += () => OnAddBusOnlyBarrierPressed();
        Debug.LogError("Button5 Added");
        addBarrierRoot.Q<Button>("AllowBusTaxiButton").clicked += () => OnAddBusandTaxiOnlyBarrierPressed();
        Debug.LogError("All Buttons Added");
    }


    private void OnAddBarrierMenuPressed()
    {
        // Activate the Add Barrier UI
        addBarrierUIDocument.gameObject.SetActive(true);
        mainUIDocument.gameObject.SetActive(false); // Deactivate the main UI
        SetupAddBarrierButtons();
    }

    private void OnBackButtonPressed()
    {
        // Activate the Main UI
        mainUIDocument.gameObject.SetActive(true);
        addBarrierUIDocument.gameObject.SetActive(false); // Deactivate the Add Barrier UI
        SetupMainButtons();
    }

    /**

    private void ChangeBarrierType(BarrierType type)
    {
        selectedBarrierType = type;
        UpdateInstructionLabel($"{type} Barrier selected.");
    }
    **/

    private void UpdateInstructionLabel(string message)
    {
        if (instructionLabel != null)
        {
            instructionLabel.text = message;
        }
    }

    // Implement the Play, Delete, Save, and Sensor methods similar to the above method templates.

    void Start()
    {
        // Get the CameraMovement component
        cameraMovement = FindObjectOfType<CameraMovement>();

        barrierManager = BarrierManager.Instance;
        if (barrierManager == null)
        {
            Debug.LogError("No BarrierManager found assigned to the BarrierButton.");
        }

        sensorManager = SensorManager.Instance;
        if (sensorManager == null)
        {
            Debug.LogError("No SensorManager found assigned to the SensorButton.");
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

        if (sensorManager != null)
        {
            List<Sensor> sensors = new List<Sensor>();
            Debug.Log("SensorManager has: " + sensorManager.allSensors);
            foreach (GameObject gameObject in sensorManager.allSensors)
            {
                Debug.Log("GameObject: " + gameObject);
                Debug.Log("Components: " + string.Join(", ", gameObject.GetComponents<Component>().Select(c => c.GetType().Name)));
                Sensor sensor = gameObject.GetComponent<Sensor>();
                if (sensor != null)
                {
                    sensors.Add(sensor);
                }
            }
            Debug.Log("Saving sensors: " + string.Join(", ", sensors.Select(s => s.name)));
            SaveSystem.SaveSensors(sensors);
            Debug.Log("Game Saved");
        }
        else
        {
            Debug.LogError("No SensorManager found in the scene.");
        }
    }

    public void OnDeleteBarrierPressed()
    {
        UpdateInstructionLabel("Click on desired barrier to delete");
        deleteMode = true;
    }

    public void OnDeleteSensorPressed()
    {
        UpdateInstructionLabel("Click on desired sensor to delete");
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

    public void OnAddSensorPressed()
    {
        sensorManager = SensorManager.Instance;
        UpdateInstructionLabel("Click on desired sensor location");
        SpawnSensor = true;
    }

    public void OnAddBarrierPressed(BarrierType barrierType)
    {
        // Set the selected barrier type
        this.selectedBarrierType = barrierType;

        UpdateInstructionLabel("Click on desired barrier location");
        SpawnBarrier = true;
    }
    public void OnAddBlockAllBarrierPressed()
    {
        OnAddBarrierPressed(BarrierType.BlockAll);
    }

    public void OnAddBlockAllMotorVehiclesBarrierPressed()
    {
        OnAddBarrierPressed(BarrierType.BlockAllMotorVehicles); // Corrected from BlockAllMotorVehicles
    }

    public void OnAddBusandTaxiOnlyBarrierPressed()
    {
        OnAddBarrierPressed(BarrierType.BusAndTaxiOnly); // Corrected from BusAndTaxiOnly
    }

    public void OnAddBlockHeavyTrafficBarrierPressed()
    {
        OnAddBarrierPressed(BarrierType.BlockHeavyTraffic);
    }

    public void OnAddBusOnlyBarrierPressed()
    {
        OnAddBarrierPressed(BarrierType.BusOnly);
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
                        barrierManager.AddBarrier(worldPosition, this.selectedBarrierType);
                        SpawnBarrier = false;
                        UpdateInstructionLabel("To add a barrier, click on the button again. To delete a barrier, click on the delete button.");
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
        if (SpawnSensor)
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
                    Debug.Log("Sensor created at " + worldPosition);
                    if (sensorManager != null)
                    {
                        sensorManager.AddSensor(worldPosition);
                        SpawnSensor = false;
                        UpdateInstructionLabel("To add a sensor, click on the button again. To delete a sensor, click on the delete button.");
                    }
                    else
                    {
                        Debug.LogError("No SensorManager found in the scene.");
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