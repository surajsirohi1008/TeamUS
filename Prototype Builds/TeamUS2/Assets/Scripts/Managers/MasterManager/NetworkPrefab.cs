using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPrefab
{
    public GameObject Prefab;

    public string Path;

    public NetworkPrefab(GameObject obj, string path)
    {
        Prefab = obj;
        Path = ReturnPrefabPathModified(path);
    }

    private string ReturnPrefabPathModified(string path)
    {
        int extensionLength = System.IO.Path.GetExtension(path).Length;
        int additionalLenght = 10;
        int startIndex = path.ToLower().IndexOf("resources");

        if (startIndex == -1)
            return string.Empty;
        else
            return path.Substring(startIndex + additionalLenght, path.Length - (additionalLenght + startIndex + extensionLength));

    }
}
