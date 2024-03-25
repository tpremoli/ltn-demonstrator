using UnityEngine;
using UnityEngine.UIElements;

public class PlayModeButtonClicker : MonoBehaviour
{
    public UIDocument uiDocument; // Make sure this is assigned in the Inspector to your UIDocument

    private Label sliderLabel; // This will hold the reference to the slider label
    private Slider slider; // This will hold the reference to the slider itself

    private void OnEnable()
    {
        // Retrieve the root element of the UI document
        var rootVisualElement = uiDocument.rootVisualElement;

        // Connect the buttons
        rootVisualElement.Q<Button>("EditLTNbutton").clicked += () => Debug.Log("Edit LTN button pressed");
        rootVisualElement.Q<Button>("MainCameraButton").clicked += () => Debug.Log("Main Camera button pressed");
        rootVisualElement.Q<Button>("CinematicCameraButton").clicked += () => Debug.Log("Cinematic Camera button pressed");
        rootVisualElement.Q<Button>("SensorCamerasButton").clicked += () => Debug.Log("Sensor Cameras button pressed");
        rootVisualElement.Q<Button>("MoreStatisticsButton").clicked += () => Debug.Log("More Statistics button pressed");

        // Connect to the slider and slider label
        slider = rootVisualElement.Q<Slider>("Slider"); // Make sure "Slider" is the name of the slider in your UXML
        sliderLabel = rootVisualElement.Q<Label>("SliderLabel"); // Make sure "SliderLabel" is the name of the label in your UXML

        if (slider == null)
        {
            Debug.LogError("Slider is not found in the UIDocument.");
        }
        else
        {
            // If the slider is found, attach a change event listener to it
            slider.RegisterValueChangedCallback(evt =>
            {
                // When the slider's value changes, update the label and log the value
                sliderLabel.text = $"Speed: {evt.newValue:F2}";
                Debug.Log($"Slider value changed to: {evt.newValue}");
            });
        }

        if (sliderLabel == null)
        {
            Debug.LogError("SliderLabel is not found in the UIDocument.");
        }
    }
}
