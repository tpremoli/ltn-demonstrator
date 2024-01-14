using UnityEngine;
using TMPro;

public class BarrierButton : MonoBehaviour
{
    public GameObject barrierPrefab;
    public TextMeshProUGUI instructionText;
    private bool SpawnBarrier = false;

    public void OnClick()
    {
        instructionText.text = "Click on desired barrier location";
        SpawnBarrier = true;
    }

    void Update()
    {
        // Check if user has clicked on screen
        if (SpawnBarrier && Input.GetMouseButtonDown(0))
        {
            // Spawn barrier at mouse position
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10; // Set this to be the distance you want the object to be placed in front of the camera.
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Instantiate(barrierPrefab, worldPosition, Quaternion.identity);
            SpawnBarrier = false;
        }
    }
}