using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManagerScript : MonoBehaviour
{
    [SerializeField] EventSequence initialSequence;
    [SerializeField] int initialPriority = -1;
    [SerializeField] AudioSource voiceLinePlayer;
    [SerializeField] float maxSubtitleTime;
    [SerializeField] SubtitleFadeOutScript subtitleDisplay;
    [SerializeField] TMP_Text subtitleText;
    List<EventSequence> queue = new List<EventSequence>();
    List<int> queuePriority = new List<int>();
    EventSequence currentSequence;
    int currentPriority;
    float timer = 0f;
    int currentEvent = 0;
    int currentSub = -1;

    Subtitles currentSubtitles;
    float subtitleTimer;

    public static DialogueManagerScript instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        currentSequence = initialSequence;
        currentPriority = initialPriority;
        runEvent();
    }
    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            runEvent();
        }
        else
        {
            timer -= Time.deltaTime;
        }

        if (voiceLinePlayer.isPlaying)
        {
            float currentTime = voiceLinePlayer.time;
            for (int i = 0; i < currentSubtitles.textTimings.Length; i++)
            {
                if (currentTime > currentSubtitles.textTimings[i] && i > currentSub)
                {
                    currentSub = i;
                    subtitleDisplay.displayText();
                    subtitleText.text = currentSubtitles.text[i];
                }
            }
        }
        else
        {
            if (subtitleTimer < maxSubtitleTime)
            {
                subtitleTimer += Time.deltaTime;
            }
            else
            {
                subtitleDisplay.fadeOut();
            }
        }
    }

    public void runEvent()
    {
        currentEvent++;
        if (currentEvent > currentSequence.events.Count || currentEvent == 0)
        {
            if (queue.Count > 0)
            {
                currentEvent = 0;
                currentSequence = queue[0];
                currentPriority = queuePriority[0];
                queue.RemoveAt(0);
                queuePriority.RemoveAt(0);
            }
            else
            {
                currentEvent = -1;
                return;
            }
        }

        DialogueEvent dialogueEvent = currentSequence.events[currentEvent - 1];

        if (dialogueEvent.isDialogue)
        {
            //Dialogue script here
            currentSubtitles = dialogueEvent.subtitles;
            currentSub = -1;
            voiceLinePlayer.clip = dialogueEvent.audio;
            voiceLinePlayer.Play();
        }
        else
        {
            dialogueEvent.unityEvent.Invoke();
        }

        timer = dialogueEvent.durationSeconds;
    }

    public void AddEventToQueue(EventSequence sequence, bool overridePriority = false, bool overrideCurrent = false)
    {
        if (overrideCurrent && overridePriority)
        {
            cancelSequence();
            currentSequence = sequence;
            currentPriority = -1;
        }
        else if (overridePriority)
        {
            queue.Insert(0, sequence);
            queuePriority.Insert(0, -1);
        }
        else
        {
            queue.Add(sequence);
            queuePriority.Add(0);
        }
    }

    public void AddEventToQueue(EventSequence sequence, int priority, bool overrideCurrent = false)
    {
        if (overrideCurrent && currentPriority != -1 && currentPriority < priority)
        {
            currentSequence = sequence;
            currentPriority = priority;
            return;
        }
        for (int i = 0; i < queue.Count; i++)
        {
            if (queuePriority[i] != -1 && queuePriority[i] < priority)
            {
                queue.Insert(i, sequence);
                queuePriority.Insert(i, priority);
                break;
            }
        }
    }

    public void cancelSequence()
    {

    }
}