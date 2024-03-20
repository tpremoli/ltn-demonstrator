using System.Collections;
using System. Collections .Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class worldSpaceUI: MonoBehaviour
{
    VisualElement root;
    private void OnEnable()
    {
        root = GetComponent<UIDocument>() .rootVisualElement;
        //root.Q<Button>("button_yes").clicked += () => Debug.Log("Ohh thank you sooo much!!"); 
        //root.Q<Button>("button_no").clicked += () => Debug.Log("You son of a monkey!");
    }
}