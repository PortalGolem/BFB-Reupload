using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForceCrash : MonoBehaviour
{
    bool triggeredDeath = false;
    [SerializeField] AudioSource deathAudio;
    void OnCollisionEnter(Collision collision)
    {
        if (!triggeredDeath)
        {
            triggeredDeath = true;
            FindFirstObjectByType<PlayerInput>().DeactivateInput();
            Time.timeScale = 0;
            deathAudio.Play();
        }
    }

    void Update()
    {
        if (triggeredDeath && !deathAudio.isPlaying)
        {
            Application.Quit();
        }
    }
}
