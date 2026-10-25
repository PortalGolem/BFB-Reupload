using UnityEngine;
using UnityEngine.SceneManagement;

public class CryoChamberScript : MonoBehaviour
{
    [SerializeField] float cooldownTime;
    bool hasPlayerEntered = false;
    float startTime = 0;

    void Start()
    {
        startTime = Time.time;
    }
    void OnTriggerEnter(Collider other)
    {
        playerControllerScript playerControllerScript = other.GetComponent<playerControllerScript>();
        if (playerControllerScript && Time.time - startTime > cooldownTime)
        {
            //triggerAnimation

            hasPlayerEntered = true;
        }
    }

    void Update()
    {
        if (hasPlayerEntered)
        {
            // if animation is done:
            //  Execute any save information here
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
