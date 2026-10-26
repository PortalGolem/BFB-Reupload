using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScript : MonoBehaviour
{
    private CanvasGroup deathScreenUI;

    [SerializeField] AudioListener audioListener;
    [SerializeField] float deathScreenFadeInTime;
    [SerializeField] float deathAudioFadeInTime;
    [SerializeField] float deathAudioFadeInOffset;

    float timer;

    void Start()
    {
        deathScreenUI = gameObject.GetComponent<CanvasGroup>();
        if (deathAudioFadeInOffset >= 0)
        {
            //This timer is as long as either the death audio fade in time or the 
            timer = Mathf.Max(deathAudioFadeInOffset + deathAudioFadeInTime, deathScreenFadeInTime);
        }
        else
        {
            timer = Mathf.Max(deathAudioFadeInTime, deathScreenFadeInTime - deathAudioFadeInOffset);
        }

    }
    bool playerKilled = false;

    public void killPlayer()
    {
        playerKilled = true;
    }

    void Update()
    {
        if (playerKilled && timer > 0)
        {
            //Come back to this, this is definitely wrong.
            timer -= Time.deltaTime;
            if (deathAudioFadeInOffset < 0)
            {
                deathScreenUI.alpha = Mathf.Clamp(1 - timer / deathScreenFadeInTime, 0f, 1f);
                AudioListener.volume = Mathf.Clamp((1 - (timer - deathAudioFadeInOffset) / deathAudioFadeInTime) / timer, 0f, 1f);
            }
            else
            {
                deathScreenUI.alpha = Mathf.Clamp(1 - (timer - deathAudioFadeInOffset) / deathScreenFadeInTime, 0f, 1f);
                AudioListener.volume = Mathf.Clamp(1 - timer / deathAudioFadeInTime, 0f, 1f);
            }
        }
    }

    public void restartDay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AudioListener.volume = 1.0f;
    }
}
