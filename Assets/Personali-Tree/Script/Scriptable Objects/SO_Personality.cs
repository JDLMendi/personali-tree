using System;
using UnityEngine;

[Serializable]
public enum Personality {Happy, Sad, Angry, Hungry, TrumpLike}

[CreateAssetMenu(fileName = "SO_Personality", menuName = "Personality")]
public class SO_Personality : ScriptableObject
{
    public Personality personality;
    public Sprite faceSprite;
    public string persona;
    public string greetings;
}
