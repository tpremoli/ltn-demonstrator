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
        rootVisualElement.Q<Button>("EditLTNbutton").clicked += () => OnEditLTNButtonPressed();
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
                Time.timeScale = evt.newValue;
                Debug.Log("Time scale is now " + Time.timeScale);
            });
        }

        if (sliderLabel == null)
        {
            Debug.LogError("SliderLabel is not found in the UIDocument.");
        }
    }

    public void OnEditLTNButtonPressed()
    {
        // Load the EditLTN scene
        Debug.Log("EditLTNButton.cs: OnButtonPressed(");
        UnityEngine.SceneManagement.SceneManager.LoadScene("EditLTN");
    }

    private float previousRealTime; // This will keep track of the real time since the last update

    private void Start()
    {
        // Initialize the previousRealTime with the current real time
        previousRealTime = Time.realtimeSinceStartup;
    }

    public void Update()
    {
        // Calculate the real time passed since the last frame
        float realDeltaTime = Time.realtimeSinceStartup - previousRealTime;
        previousRealTime = Time.realtimeSinceStartup;

        // Log the message with the time scale and the real delta time
        Debug.Log($"Update called with time scale: {Time.timeScale} | Real Delta Time: {realDeltaTime:F3}");
    }

}
