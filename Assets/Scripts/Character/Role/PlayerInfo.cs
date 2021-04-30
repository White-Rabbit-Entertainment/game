using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PlayerInfo : MonoBehaviour {
    public string colorName;
    public Avatar avatar;
    public Color color;
    public GameObject modelPrefab;
    public string assetPath;

    void Reset() {
        assetPath = AssetDatabase.GetAssetPath(gameObject);
        Debug.Log(assetPath);
        assetPath = assetPath.Replace(".prefab", "");
        assetPath = assetPath.Replace("Assets/Resources/", "");
        Debug.Log(assetPath);
    }

    public static PlayerInfo Get(string assetPath) {
        Debug.Log(assetPath);
        return Resources.Load<GameObject>(assetPath).GetComponent<PlayerInfo>();
    }

    public PlayerInfo(string colorName, Avatar avatar, Color color, GameObject modelPrefab) {
        this.colorName = colorName;
        this.avatar = avatar;
        this.color = color;
        this.modelPrefab = modelPrefab;
    }
}