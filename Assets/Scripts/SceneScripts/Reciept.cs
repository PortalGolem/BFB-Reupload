using TMPro;
using UnityEngine;

public class Reciept : Interactable
{
    public static int NotePosition = 0;

    bool hasInteracted;

    [SerializeField] Vector3 NoteDistance;
    [SerializeField] Vector3 NoteStartPosition;
    [SerializeField] Quaternion HangingRotation;
    [SerializeField] RectTransform note;
    public string text;

    public override bool Interact()
    {
        if (!hasInteracted)
        {
            hasInteracted = true;
            transform.SetPositionAndRotation(NoteStartPosition + NoteDistance * NotePosition, HangingRotation);
        }

        RectTransform noteInstance = NoteDisplay.instance.DisplayNote(note);
        noteInstance.GetComponentInChildren<TMP_Text>().text = text;

        return true;
    }
}
