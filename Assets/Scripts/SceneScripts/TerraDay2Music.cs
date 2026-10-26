using UnityEngine;

public class TerraDay2Music : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicPlayerScript.instance.fade("goodmourning");
    }
}
