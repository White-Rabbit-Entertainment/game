using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class PlayerInfo : MonoBehaviour {
    public string colorName;
    public Avatar avatar;
    public Color color;
    public string assetPath;

    void Reset() {
        assetPath = AssetDatabase.GetAssetPath(gameObject);
        Debug.Log(assetPath);
        assetPath = assetPath.Replace(".prefab", "");
        assetPath = assetPath.Replace("Assets/Resources/", "");
        Debug.Log(assetPath);
    }

    public static PlayerInfo Get(string assetPath) {
        return Resources.Load<GameObject>(assetPath).GetComponent<PlayerInfo>();
    }
    
    public static GameObject GetPrefab(string assetPath) {
        return Resources.Load<GameObject>(assetPath);
    }

    public PlayerInfo(string colorName, Avatar avatar, Color color) {
        this.colorName = colorName;
        this.avatar = avatar;
        this.color = color;
    }
}
