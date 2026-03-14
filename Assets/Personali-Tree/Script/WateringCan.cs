using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class WateringCan : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public TreeManager treeManager;

    public void Start()
    {
        treeManager = FindFirstObjectByType<TreeManager>();
    }

    [Button(ButtonSizes.Small)]
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Watering Can Pressed");
        treeManager.WaterPlant();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Watering Can Enter");
    }
    
    
}

