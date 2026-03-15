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
    public TreeObject currentTree;
    public string url = "http://127.0.0.1:5000/chat";
    
    public RawImage faceImage;
    public SO_Personality personality;
    public TreeManager treeManager;

    public GameObject chatBubble;
    public TMP_Text chatText;
    public CanvasGroup chatCanvasGroup;
    
    private Coroutine _chatCoroutine;
    
    private void Start()
    {
        treeManager = FindFirstObjectByType<TreeManager>();
        treeManager.onProcessText.AddListener(TakeInput);
        currentTree.shaken.AddListener(OnShaken);
        
        ShowGreeting(personality.greetings);
    }

    private void OnShaken()
    {
        ShakeApple(strength: 0.1f, vibrato: 5, randomness: 20f);
        SendToAPI("In less than three words, your tree has been shaken!", 5.0f);
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
    
    private void ShowGreeting(string message)
    {
        Debug.Log($"{personality.personality} greets you: {message}");
        chatText.text = message;
        
        // Use a 5-second display for the initial greeting
        if (_chatCoroutine != null) StopCoroutine(_chatCoroutine);
        _chatCoroutine = StartCoroutine(DisplayTempChat(5f));
    }

 
    public void TakeInput(string input)
    {
        Debug.Log($"{personality.personality} has gotten the input: {input}");
        SendToAPI(input, 15.0f);
    }

    private void SendToAPI(string input, float time)
    {
        Debug.Log("Sending to Server");
        StartCoroutine(Post(input, time));
    }

    private void ProcessResult(string input, float time)
    {
        Debug.Log($"{personality.personality} says: {input}");
        chatText.text = input;
        StartCoroutine(DisplayTempChat(time));
    }

    // private void OutputResults()
    // {
    //     return;
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Heavy shake: Higher strength, more vibrato
        ShakeApple(strength: 0.1f, vibrato: 5, randomness: 20f);
        var input = "You've been tapped on!";
        SendToAPI(input, 5.0f);
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
        
        // If you added a CanvasGroup, fade it in
        if (chatCanvasGroup != null)
        {
            chatCanvasGroup.alpha = 0;
            chatCanvasGroup.DOFade(1, 0.5f);
        }

        yield return new WaitForSeconds(second);

        // Fade out before disabling
        if (chatCanvasGroup != null)
        {
            yield return chatCanvasGroup.DOFade(0, 0.5f).WaitForCompletion();
        }
        
        chatBubble.SetActive(false);
    }


    #region Python Connection

    private IEnumerator Post(string input, float seconds)
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
                ProcessResult(response.reply, seconds);
            }
            else
            {
                Debug.LogError($"API Error: {request.error}");
            }
        }

    }

    #endregion
}
