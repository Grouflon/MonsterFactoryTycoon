using UnityEngine;
using System.Collections;

public class UnityTools
{
    static public void DestroyAllChildren(Transform _transform)
    {
        foreach (Transform child in _transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
