using UnityEngine;
using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

[Serializable]
public class TreeStage
{
    public string name;         // Just for organization in the Inspector
    public GameObject model;    // The visual for this stage
    public int waterRequired;   // How much water to finish THIS stage
}

public class TreeManager : MonoBehaviour
{
    public TreeObject currentTree;
    public TMP_InputField userInput;
    
    [Header("Personalities")]
    public SO_Personality[] personalities;
    
    [Header("Growth Configuration")]
    public TreeStage[] stages; 
    
    [Header("Current Status")]
    public int currentStageIndex = 0;
    public int currentWater = 0;

    [Header("Events")] public UnityEvent<string> onProcessText;
    private void Start()
    {
        UpdateVisuals();
    }
    
    private void Update()
    {
        // Use Keyboard.current to check for the Enter key
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log(userInput.text);
            if (!string.IsNullOrEmpty(userInput.text))
            {
                onProcessText?.Invoke(userInput.text);
                userInput.text = "";
            }
        }
    }

    [Button(ButtonSizes.Small)]
    public void WaterPlant()
    {
        if (currentStageIndex >= stages.Length - 1)
        {
            Debug.Log("The tree is at its final form!");
            return;
        }

        currentWater++;
        
        int needed = stages[currentStageIndex].waterRequired;

        if (currentWater >= needed)
        {
            GrowToNextStage();
        }
    }

    private void GrowToNextStage()
    {
        currentWater = 0;
        currentStageIndex++;
        UpdateVisuals();
        
        Debug.Log($"Grown! Now at stage: {stages[currentStageIndex].name}");
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            var  stage = stages[i];
            if (stage.model != null)
            {
                stage.model.SetActive(i == currentStageIndex);
                currentTree = stage.model.GetComponent<TreeObject>();
                currentTree.StartTree();
            }
        }
    }

}