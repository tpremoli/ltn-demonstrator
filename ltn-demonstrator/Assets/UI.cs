using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        // UIBuild_1.uxml
        /*
        <ui:VisualElement name="container" style="flex-grow: 1; flex-basis: 20%; position: absolute; height: 114px; width: 912px; top: 367px; color: rgb(0, 0, 0); -unity-text-outline-color: rgba(0, 0, 0, 0); background-color: rgba(231, 226, 191, 0.53); left: -6px; border-left-color: rgb(0, 0, 0); border-right-color: rgb(0, 0, 0); border-top-color: rgb(0, 0, 0); border-bottom-color: rgb(0, 0, 0); border-top-width: 5px; border-right-width: 5px; border-bottom-width: 5px; border-left-width: 5px; flex-direction: row;">
                <ui:Button text="Edit LTN" parse-escape-sequences="true" display-tooltip-when-elided="true" class="button" style="flex-direction: row-reverse; white-space: nowrap; align-items: auto; justify-content: flex-end; -unity-text-align: upper-center; height: 45px; width: 91px;" />
                <ui:Button text="Main Camera" parse-escape-sequences="true" display-tooltip-when-elided="true" class="button" style="flex-direction: row-reverse; white-space: nowrap; align-items: auto; justify-content: flex-end; -unity-text-align: upper-center; height: 45px; width: 143px;" />
                <ui:Button text="Cinematic Camera" parse-escape-sequences="true" display-tooltip-when-elided="true" class="button" style="flex-direction: row-reverse; white-space: nowrap; align-items: auto; justify-content: flex-end; -unity-text-align: upper-center; height: 45px; width: 143px;" />
                <ui:Button text="Sensor Cameras" parse-escape-sequences="true" display-tooltip-when-elided="true" class="button" style="flex-direction: row-reverse; white-space: nowrap; align-items: auto; justify-content: flex-end; -unity-text-align: upper-center; height: 45px; width: 143px;" />
                <ui:Button text="More Statistics" parse-escape-sequences="true" display-tooltip-when-elided="true" class="button" style="flex-direction: row-reverse; white-space: nowrap; align-items: auto; justify-content: flex-end; -unity-text-align: upper-center; height: 45px; width: 143px;" />
                <ui:Label tabindex="-1" text="Statistics:" parse-escape-sequences="true" display-tooltip-when-elided="true" style="width: 191px;" />
        </ui:VisualElement>
        */
        Button EditLTNbutton = root.Q<Button>("EditLTNbutton");
        // Camera Buttons
        Button MainCameraButton = root.Q<Button>("MainCameraButton");
        Button CinematicCameraButton = root.Q<Button>("CinematicCameraButton");
        Button SensorCamerasButton = root.Q<Button>("SensorCamerasButton");
        // Statistics Button
        Button MoreStatisticsButton = root.Q<Button>("MoreStatisticsButton");
        // Statistics Label
        Label StatisticsLabel = root.Q<Label>("StatisticsLabel");

        // Add click events
        EditLTNbutton.clicked += () => EditLTN();
        MainCameraButton.clicked += () => MainCamera();
        CinematicCameraButton.clicked += () => CinematicCamera();
        SensorCamerasButton.clicked += () => SensorCameras();
        MoreStatisticsButton.clicked += () => MoreStatistics();

        // Set Statistics Label
        StatisticsLabel.text = "Statistics:"; // replace with ted's code
    }

    private void EditLTN()
    {
        Debug.Log("Edit LTN");
    }

    private void MainCamera()
    {
        Debug.Log("Main Camera");
    }

    private void CinematicCamera()
    {
        Debug.Log("Cinematic Camera");
    }

    private void SensorCameras()
    {
        Debug.Log("Sensor Cameras");
    }

    private void MoreStatistics()
    {
        Debug.Log("More Statistics");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update Statistics Label");
        
    }
}
