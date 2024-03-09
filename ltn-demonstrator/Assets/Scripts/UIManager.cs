using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class UIManager : MonoBehaviour
{
    public GameObject sensorStatsPanel;
    public TextMeshProUGUI sensorStatsText;

    public void ShowSensorStats(Vector3 position, int sensor_trav_count)
    {
        sensorStatsPanel.SetActive(true);
        sensorStatsPanel.transform.position = position;
        sensorStatsText.text = "Sensor Trav Count: " + sensor_trav_count;
    }

    public void HideSensorStats()
    {
        sensorStatsPanel.SetActive(false);
    }
}