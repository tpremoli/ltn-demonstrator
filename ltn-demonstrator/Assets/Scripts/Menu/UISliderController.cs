using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UISliderController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI sliderValueText;

    [SerializeField]
    private float maxSliderValue = 100f;

    public void OnSliderValueChanged(float value)
    {
        Debug.Log("value: " + value);
        Debug.Log("maxSliderValue: " + maxSliderValue);

        float localValue = value * maxSliderValue;
        Debug.Log("localValue: " + localValue);

        sliderValueText.text = localValue.ToString();

        // Change the time scale with the slider value
        Time.timeScale = localValue;
    }
}