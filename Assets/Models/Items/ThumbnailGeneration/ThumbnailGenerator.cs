using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ThumbnailGenerator : MonoBehaviour
{
    [SerializeField] int thumbnailWidth = 256;
    [SerializeField] int thumbnailHeight = 256;
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] Transform itemPosition;
    [SerializeField] Camera thumbnailCamera;
    public ItemList itemsToRender;
    public List<Sprite> textures;
    private GameObject currentItem;

    public static ThumbnailGenerator instance;

    void Awake()
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        updateAllThumbnails();
        #if UNITY_EDITOR
        
        SceneManager.LoadScene(SceneData.instance.testSceneName);

        #else

        SceneManager.LoadScene(1);

        #endif
    }


    private void LoadObject(GameObject item)
    {
        if (currentItem != null) { 
            currentItem.SetActive(false);
        }
        currentItem = Instantiate(item, itemPosition.position, itemPosition.rotation);
    }
    
    private void RenderThumbnail(int index)
    {
        thumbnailCamera.Render();

        RenderTexture.active = renderTexture;

        Texture2D tex = new Texture2D(thumbnailWidth, thumbnailHeight, TextureFormat.RGBA32, false);
        
        tex.ReadPixels(new Rect(0, 0, thumbnailWidth, thumbnailHeight), 0, 0);

        tex.Apply();

        textures.Add(Sprite.Create(tex, new Rect(0, 0, thumbnailWidth, thumbnailHeight), new Vector2(.5f, .5f), 100.0f));
    }


    public bool updateAllThumbnails()
    {
        textures = new List<Sprite>(itemsToRender.items.Count);
        for (int i = 0; i < itemsToRender.items.Count; i++)
        {
            LoadObject(itemsToRender.items[i].mesh);
            RenderThumbnail(i);
            itemsToRender.items[i].iconID = i;
        }

        return false;
    }
}
