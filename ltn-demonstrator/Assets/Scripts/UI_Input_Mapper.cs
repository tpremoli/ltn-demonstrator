using System. Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class UI_InputMapper: MonoBehaviour
{
    UIDocument document;
    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        // TODO here you will need to filter out only the UI document you want to work for, I think that's why your menu gets moved

        document.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPosition) => 
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100, Color.magenta);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("UI")))
            {
                Debug.Log("invalid position");
                return invalidPosition;
            }

            Vector2 pixelUV = hit.textureCoord;

            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= this.document.panelSettings.targetTexture.width;
            pixelUV.y *= this.document.panelSettings.targetTexture.height;

            var cursor = this.document.rootVisualElement.Q<VisualElement>("cursor");

            if (cursor != null)
            {
                cursor.style.left = pixelUV.x;
                cursor.style.top = pixelUV.y;
            }

            return pixelUV;
        });
    }

}