using System;
using System.Collections;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Networking;

[Serializable]
public class ChatInput
{
    public string personality;
    public string persona;
    public string input;
}

[Serializable]
public class ChatResponse {
    public string reply;
    public string status;
}

public class AppleObject : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public string url = "http://127.0.0.1:5000/chat";
    
    public RawImage faceImage;
    public SO_Personality personality;
    public TreeManager treeManager;

    public GameObject chatBubble;
    public TMP_Text chatText;
    
    private void Start()
    {
        treeManager = FindFirstObjectByType<TreeManager>();
        treeManager.onProcessText.AddListener(TakeInput);
    }

    private void OnDisable()
    {
        treeManager.onProcessText.RemoveListener(TakeInput);
    }

    private void Update()
    {
        if (personality == null || faceImage.texture == null)
        {
            faceImage.texture = personality.faceSprite.texture;
        }
    }

    public string GetResponse(string input)
    {
        return "This is a test!";
    }

    public void TakeInput(string input)
    {
        Debug.Log($"{personality.personality} has gotten the input: {input}");
        SendToAPI(input);
    }

    private void SendToAPI(string input)
    {
        Debug.Log("Sending to Server");
        StartCoroutine(Post(input));
    }

    private void ProcessResult(string input)
    {
        Debug.Log($"{personality.personality} says: {input}");
        chatText.text = input;
        StartCoroutine(DisplayTempChat(10f));
    }

    // private void OutputResults()
    // {
    //     return;
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Heavy shake: Higher strength, more vibrato
        ShakeApple(strength: 0.1f, vibrato: 5, randomness: 20f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Subtle shake: Low strength, lower vibrato
        ShakeApple(strength: 0.05f, vibrato: 5, randomness: 20f);
    }

    public void ShakeApple(float strength, int vibrato, float randomness)
    {
        transform.DOKill(true);

        // Force the local Z to a "safe" forward position before shaking
        Vector3 safePos = transform.localPosition;
        safePos.z = -0.1f; 
        transform.localPosition = safePos;

        // Use DOShakePosition
        transform.DOShakePosition(1f, strength, vibrato, randomness)
            .OnComplete(() => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0));
    }

    private IEnumerator DisplayTempChat(float second)
    {
        chatBubble.SetActive(true);
        yield return new WaitForSeconds(second);
        chatBubble.SetActive(false);
    }


    #region Python Connection

    private IEnumerator Post(string input)
    {
        ChatInput chat = new ChatInput();
        chat.personality = personality.personality.ToString();
        chat.persona = personality.persona;
        chat.input = input;
        
        string json =  JsonUtility.ToJson(chat);
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("I have received response");
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                ProcessResult(response.reply);
            }
            else
            {
                Debug.LogError($"API Error: {request.error}");
            }
        }

    }

    #endregion
}
