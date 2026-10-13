using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScript", menuName = "Scriptable Objects/Item")]
public class ItemScript : ScriptableObject
{
    [NonSerialized] public int iconID = -1;
    public GameObject mesh;
    public string Name = "";
    public GameObject heldItem;
    public GameObject droppedItem;
    public Vector3 IconPositionsOffset = Vector3.zero;
    public Vector3 iconPositionOffset = Vector3.zero;
    
    [SerializeField] Vector3 meshScaleOffset = Vector3.one;

    [SerializeField] Vector3 heldPositionOffset = new Vector3(-0.2f, -0.2394999f, 0.609f);
    [SerializeField] Vector3 heldRotationOffset = new Vector3(0, -91.072f, 0);

    public GameObject GenerateDroppedItem(Vector3 position, Quaternion rotation)
    {
        GameObject output = droppedItem == null ? new GameObject() : Instantiate(droppedItem);

        GameObject meshObject = new GameObject();
        meshObject.transform.SetParent(output.transform, false);

        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh.GetComponent<MeshFilter>().sharedMesh;

        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = mesh.GetComponent<MeshRenderer>().sharedMaterials;

        meshObject.transform.localScale = meshScaleOffset;

        BoxCollider collider = output.AddComponent<BoxCollider>();
        collider.center = meshFilter.sharedMesh.bounds.center;
        collider.size = Vector3.Scale(meshFilter.sharedMesh.bounds.size, meshScaleOffset);
        
        Rigidbody rigidbody = output.AddComponent<Rigidbody>();
        rigidbody.excludeLayers = LayerMask.GetMask("Player");

        PickUpScript pickUpScript = output.AddComponent<PickUpScript>();
        pickUpScript.item = this;

        return output;
    }

    public GameObject GenerateHeldItem(Transform transform)
    {
        GameObject output;
        if (heldItem == null){
            output = new GameObject();
            output.transform.SetParent(transform, false);
            GameObject outputMesh = Instantiate(mesh, output.transform);
            output.transform.localPosition = heldPositionOffset;
            output.transform.localRotation = Quaternion.Euler(heldRotationOffset);
            outputMesh.transform.localScale = meshScaleOffset;
        } else
        {
            output = Instantiate(heldItem, transform);
        }

        output.AddComponent<HeldItemScript>();
        return output;
    }
}
