using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    [SerializeField] Animator menuAnimator;
    public void startGame()
    {
        //Load inital scene. Scene two is after the main menu and item Icon generator scene.
        SceneManager.LoadScene(2);
    }

    public void optionsMenu()
    {
        menuAnimator.SetTrigger("OptionsMenu");
    }

    public void mainMenu()
    {
        menuAnimator.SetTrigger("MainMenu");
    }
}
