using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISliderController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI sliderValueText;

    [SerializeField]
    private Slider slider;

    // Assuming you have a variable for speed
    private float speed;

    void Start()
    {
        // Ensure the Slider component is assigned in the inspector
        if (slider != null)
        {
            slider.minValue = 1; // Set the minimum value to 1
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            Debug.LogError("Slider is not assigned in the inspector");
        }
    }

    void OnSliderValueChanged(float value)
    {
        // Update the text
        sliderValueText.text = value.ToString();

        // Update the speed
        speed = value;

        // Update the time scale
        Time.timeScale = value;

        // Log the new speed and time scale
        Debug.Log("Speed is now " + speed);
        Debug.Log("Time scale is now " + Time.timeScale);
    }
}