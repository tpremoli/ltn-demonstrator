using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class MiniWorld_UI : MonoBehaviour
{
    VisualElement root;
    private List<VisualElement> sensorUIElements;
    private SensorManager sensorManager;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        sensorManager = FindObjectOfType<SensorManager>();

        // Assuming you have a VisualTreeAsset for sensors UI
        VisualTreeAsset sensorUIAsset = Resources.Load<VisualTreeAsset>("SensorUI");
        
        // Load sensors from the sensor manager
        sensorManager.LoadSensorsFromSave();
        
        // Clear existing UI elements if any
        if(sensorUIElements != null)
        {
            foreach(var sensorUI in sensorUIElements)
            {
                root.Remove(sensorUI);
            }
        }
        
        // Initialize the list for UI elements
        sensorUIElements = new List<VisualElement>();

        // Get the sensor data list from the sensor manager
        // The SensorManager should have a public method or property to access the sensors' data
        var sensorDataList = sensorManager.GetAllSensorData(); 

        foreach (var sensorData in sensorDataList)
        {
            VisualElement sensorUI = sensorUIAsset.Instantiate();
            // Update the sensorUI based on the sensorData
            // This could involve setting text, positions, or other properties to reflect the state of the sensor
            sensorUIElements.Add(sensorUI);
            root.Add(sensorUI);
        }
    }

    // This method can be called to refresh the UI when sensors are added, removed, or changed
    public void RefreshSensorUI()
    {
        OnEnable();
    }

    private void Update()
{
    // Assuming sensorUIElements is the list that holds the UI elements for each sensor
    // and allSensors is the list that holds the actual sensor GameObjects
    for (int i = 0; i < sensorManager.allSensors.Count; i++)
    {
        GameObject sensor = sensorManager.allSensors[i];
        VisualElement sensorUI = sensorUIElements[i];

        // Convert the world position of the sensor to a screen point for the UI
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(sensor.transform.position);

        // Update the style of the sensor UI to position it on the screen
        sensorUI.style.left = screenPoint.x - (sensorUI.layout.width / 2);
        sensorUI.style.top = Screen.height - screenPoint.y - 100; // Offset by 100 pixels from the top
    }
}

}
