using System;
using System.Collections.Generic;
using UnityEngine;

public class AppleManager : MonoBehaviour
{
    public string input;
    public List<GameObject> apples = new List<GameObject>();
    public GameObject applePrefab;

    private void OnEnable()
    {
        var foundApples =  FindObjectsOfType<AppleObject>();
        foreach (var apple in foundApples)
        {
            Debug.Log($"This apple got the {apple.personality} personality");;
        }
    }

    public void ProcessInput()
    {
        foreach (var apple in apples)
        {
            // apple.GetResponse(input);
        }
    }

    public void CreateApple(SO_Personality personality, Vector3 position)
    {
        var apple= Instantiate(applePrefab, transform);
        apple.transform.parent = transform;
        apple.transform.localPosition = position;
        apples.Add(apple);
        
        var appleObj =  apple.GetComponent<AppleObject>();
        appleObj.personality = personality;
    }
}
