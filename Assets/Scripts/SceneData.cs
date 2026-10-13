#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class SceneData : ScriptableSingleton<SceneData>
{
    public string testSceneName = "";
}
#endif