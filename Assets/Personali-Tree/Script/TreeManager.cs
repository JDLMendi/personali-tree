using UnityEngine;
using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
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
    
    [Header("Transition Settings")]
    public float fadeDuration = 0.5f;
    public float scalePunchAmount = 0.2f;

    [Header("Growth Configuration")]
    public TreeStage[] stages; 
    
    [Header("Current Status")]
    public int currentStageIndex = 0;
    public int currentWater = 0;

    [Header("Events")] public UnityEvent<string> onProcessText;
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    private void Start()
    {
        // Initialize visuals without animation on start
        RecordOriginalScales();
        InitialSetup();
    }
    
    private void Update()

    {
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
    
    private void RecordOriginalScales()
    {
        foreach (var stage in stages)
        {
            if (stage.model != null)
            {
                // Save the scale you set in the Inspector
                originalScales[stage.model] = stage.model.transform.localScale;
            }
        }
    }

    private void InitialSetup()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            bool isCurrent = (i == currentStageIndex);
            stages[i].model.SetActive(isCurrent);
            
            if (isCurrent)
            {
                currentTree = stages[i].model.GetComponent<TreeObject>();
                if(currentTree != null) currentTree.StartTree();
                
                // Ensure it starts at its correct scale
                stages[i].model.transform.localScale = originalScales[stages[i].model];
            }
        }
    }

    private void GrowToNextStage()
    {
        GameObject oldModel = stages[currentStageIndex].model;
        currentStageIndex++;
        GameObject newModel = stages[currentStageIndex].model;

        PerformTransition(oldModel, newModel);
        
        currentWater = 0;
        Debug.Log($"Grown! Now at stage: {stages[currentStageIndex].name}");
    }

    private void PerformTransition(GameObject oldModel, GameObject newModel)
    {
        // 1. Scale old model down to zero
        oldModel.transform.DOScale(Vector3.zero, fadeDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() => oldModel.SetActive(false));

        // 2. Prepare new model
        newModel.SetActive(true);
        newModel.transform.localScale = Vector3.zero; 

        // 3. Scale up to its UNIQUE original scale (not just Vector3.one)
        Vector3 targetScale = originalScales[newModel];

        newModel.transform.DOScale(targetScale, fadeDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                currentTree = newModel.GetComponent<TreeObject>();
                if(currentTree != null) currentTree.StartTree();
                
                // Punch scale relative to its natural size
                newModel.transform.DOPunchScale(targetScale * scalePunchAmount, 0.3f);
            });
    }

    [Button(ButtonSizes.Small)]
    public void WaterPlant()
    {
        if (currentStageIndex >= stages.Length - 1) return;

        currentWater++;
        if (currentWater >= stages[currentStageIndex].waterRequired)
        {
            GrowToNextStage();
        }
    }
}