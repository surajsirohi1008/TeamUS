using UnityEngine;

public static class Transforms
{
    public static void DestroyChildren(this Transform t, bool destroyimmediately = false)
    {
        foreach (Transform child in t)
        {
            if (destroyimmediately)
                MonoBehaviour.DestroyImmediate(child.gameObject);
            else
                MonoBehaviour.Destroy(child.gameObject);
        }
    }
}
