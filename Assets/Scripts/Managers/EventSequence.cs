using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventSequence", menuName = "EventSequence")]
public class EventSequence : ScriptableObject
{
    public List<DialogueEvent> events = new List<DialogueEvent>();
}


[Serializable]
public class DialogueEvent
{
    public bool isDialogue = true;
    public float durationSeconds = 1.0f;
    public UnityEvent unityEvent;
    public Subtitles subtitles;
    public AudioClip audio;
}
