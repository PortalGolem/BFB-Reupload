using UnityEngine;

public class ItemSpawnerScript : MonoBehaviour
{
    [SerializeField] ItemScript item;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject createdItem = item.GenerateDroppedItem(transform.position, transform.rotation);
        createdItem.transform.position = transform.position;
        createdItem.transform.rotation = transform.rotation;
    }
}
