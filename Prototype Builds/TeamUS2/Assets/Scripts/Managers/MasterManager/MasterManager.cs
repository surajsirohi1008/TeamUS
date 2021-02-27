using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "Singletons/MasterManager")]

public class MasterManager : SingletonScriptableObject<MasterManager>
{
    [SerializeField]
    private GameSettings _gameSettings;

    public static GameSettings GameSettings { get { return Instance._gameSettings; } }

    private List<NetworkPrefab> _networkPrefabs = new List<NetworkPrefab>();

    public static GameObject NetworkInstantiate(GameObject obj, Vector3 position, Quaternion rotation)
    {
        foreach (NetworkPrefab networkPrefab in Instance._networkPrefabs)
        {
            if (networkPrefab.Prefab == obj)
            {
                if (networkPrefab.Path != string.Empty)
                {
                    GameObject result = PhotonNetwork.Instantiate(networkPrefab.Path, position, rotation);
                    return result;
                }
                else
                {
                    Debug.LogError("Path is empty for Gameobject Name: " + networkPrefab.Prefab);
                    return null;
                }

            }
        }

        return null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PopulateNetworkPrefab()
    {
#if UNITY_EDITOR

        Instance._networkPrefabs.Clear();

        GameObject[] results = Resources.LoadAll<GameObject>("");
        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].GetComponent<PhotonView>() != null)
            {
                string path = AssetDatabase.GetAssetPath(results[i]);
                Instance._networkPrefabs.Add(new NetworkPrefab(results[i], path));
            }
        }

        for (int i = 0; i < Instance._networkPrefabs.Count; i++)
        {
            UnityEngine.Debug.Log("Checking Populator "+Instance._networkPrefabs[i].Prefab.name + ", " + Instance._networkPrefabs[i].Path);
        }

#endif
    }
}
