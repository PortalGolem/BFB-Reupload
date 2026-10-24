using UnityEngine;
using UnityEngine.InputSystem;

public class NoteDisplay : MonoBehaviour
{
    [SerializeField] playerControllerScript playerController;

    bool exited = false;

    public static NoteDisplay instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    RectTransform currentNote;
    Animator currentAnimator;

    void Start()
    {
        currentAnimator = gameObject.GetComponent<Animator>();
    }

    public RectTransform DisplayNote(RectTransform note)
    {
        Debug.Log("CreatedNote!");
        currentNote = Instantiate(note, transform.GetChild(0));
        currentNote.gameObject.SetActive(true);
        currentAnimator.SetTrigger("StartAnimation");
        currentAnimator.ResetTrigger("Exit");
        playerController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        exited = false;
        return currentNote;
    }

    //Hello

    // Update is called once per frame
    void Update()
    {
        if (exited && currentAnimator.GetCurrentAnimatorStateInfo(0).IsName("TemplatePreMove") && currentNote != null)
        {
            Destroy(currentNote.gameObject);
        }
        if (
            Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame
            )
        {
            currentAnimator.SetTrigger("Exit");
            playerController.enabled = true;
            exited = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
