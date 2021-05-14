using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PlayerInfo : MonoBehaviour {
    public string colorName;
    public Avatar avatar;
    public Color color;
    public string assetPath;
    public GameObject ghostPrefab;
    public Outline outline;
    public GameObject localHiddenMesh;

    #if UNITY_EDITOR
        void Reset() {
            assetPath = AssetDatabase.GetAssetPath(gameObject);
            assetPath = assetPath.Replace(".prefab", "");
            assetPath = assetPath.Replace("Assets/Resources/", "");
        }
    #endif

    public static PlayerInfo Get(string assetPath) {
        return Resources.Load<GameObject>(assetPath).GetComponent<PlayerInfo>();
    }
    
    public static GameObject GetPrefab(string assetPath) {
        return Resources.Load<GameObject>(assetPath);
    }
}
