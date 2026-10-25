using UnityEngine;

[CreateAssetMenu(fileName = "MusicObject", menuName = "Scriptable Objects/MusicObject")]
public class MusicObject : ScriptableObject
{
    public string songName;
    public int BPM;
    public AudioClip songFile;
}
