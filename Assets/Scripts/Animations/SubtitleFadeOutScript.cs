using NUnit.Framework;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleFadeOutScript : MonoBehaviour
{
    [SerializeField] AnimationCurve fadeOutCurve;
    [SerializeField] float initialPanelOpacity;
    [SerializeField] float initialTextOpacity;
    [SerializeField] float fadeOutTime;
    [SerializeField] Image panel;
    [SerializeField] TMP_Text text;
    float fadeOutTimer;
    bool isFadingOut;

    // Update is called once per frame
    void Update()
    {
        if (fadeOutTimer < fadeOutTime && isFadingOut)
        {
            fadeOutTimer += Time.deltaTime;
            float opacityPercent = fadeOutCurve.Evaluate(fadeOutTimer / fadeOutTime);
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, math.max(opacityPercent * initialPanelOpacity, 0));
            text.color = new Color(text.color.r, text.color.g, text.color.b, math.max(opacityPercent * initialTextOpacity, 0));
        }
        else if (!isFadingOut)
        {
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, initialPanelOpacity);
            text.color = new Color(text.color.r, text.color.g, text.color.b, initialTextOpacity);
        }
    }

    public void fadeOut()
    {
        isFadingOut = true;
        fadeOutTimer = 0;
    }

    public void displayText()
    {
        isFadingOut = false;
    }
}
