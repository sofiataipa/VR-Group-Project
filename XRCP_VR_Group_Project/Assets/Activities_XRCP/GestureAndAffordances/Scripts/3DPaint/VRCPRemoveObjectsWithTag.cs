using UnityEngine;

/// <summary>
/// Removes all objects in a scene using a particular tag
/// </summary>
public class VRCPRemoveObjectsWithTag : MonoBehaviour
{
    public void RemoveObjects(string tag)
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject targetObject in taggedObjects)
            Destroy(targetObject);  
    }
}
