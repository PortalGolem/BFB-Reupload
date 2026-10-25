using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    float timeTillTestTransition = 10;
    bool isTrans = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicPlayerScript.instance.fade("9-5tillheavenspunishment");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTrans)
        {
            timeTillTestTransition -= Time.deltaTime;
            if (timeTillTestTransition < 0)
            {
                isTrans = true;
                MusicPlayerScript.instance.fade("consternation");
            }
        }
    }
}
