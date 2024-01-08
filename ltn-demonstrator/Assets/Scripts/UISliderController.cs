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
        float localValue = value * maxSliderValue;
        sliderValueText.text = localValue.ToString();

        // Change the time scale with the slider value
        Time.timeScale = localValue;
    }

    
}