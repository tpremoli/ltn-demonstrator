using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonClicker : MonoBehaviour
{
    UIDocument buttonDocument;
    Label instructionLabel;

    void OnEnable()
    {
        buttonDocument = GetComponent<UIDocument>();

        if (buttonDocument == null)
        {
            Debug.LogError("No button document found");
            return;
        }

        // Retrieve the label by its name
        instructionLabel = buttonDocument.rootVisualElement.Q<Label>("InstructionLabel");

        // Setup buttons with their event listeners
        SetupButton("PlayButton", "Play Button was pressed");
        SetupButton("AddBarriersButton", "Add Barriers Button was pressed");
        SetupButton("DeleteBarrierButton", "Delete Barrier Button was pressed");
        SetupButton("DeleteSaveButton", "Delete Save Button was pressed");
        SetupButton("AddSensorsButton", "Add Sensors Button was pressed");
    }

    private void SetupButton(string buttonName, string message)
    {
        Button button = buttonDocument.rootVisualElement.Q<Button>(buttonName);
        if (button != null)
        {
            Debug.Log("BUTTON FOUND");
            button.clicked += () => UpdateLabel(message);
        }
        else
        {
            Debug.LogError($"Button {buttonName} not found in the document");
        }
    }

    private void UpdateLabel(string message)
    {
        if (instructionLabel != null)
        {
            instructionLabel.text = message;
        }
        else
        {
            Debug.LogError("Instruction label not found in the document");
        }
    }
}
