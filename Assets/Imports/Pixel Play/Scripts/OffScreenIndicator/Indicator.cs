using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Assign this script to the indicator prefabs.
/// </summary>
public class Indicator : MonoBehaviour
{
    [SerializeField] private IndicatorType indicatorType;
    [SerializeField] private TextMeshProUGUI titleText;
    private Image indicatorImage;
    private TextMeshProUGUI distanceText;
    private Sprite defaultImage;
    private string defaultText;


    /// <summary>
    /// Gets if the game object is active in hierarchy.
    /// </summary>
    public bool Active
    {
        get
        {
            return transform.gameObject.activeInHierarchy;
        }
    }

    /// <summary>
    /// Gets the indicator type
    /// </summary>
    public IndicatorType Type
    {
        get
        {
            return indicatorType;
        }
    }

    void Awake()
    {
        indicatorImage = transform.GetComponent<Image>();
        distanceText = transform.GetComponentInChildren<TextMeshProUGUI>();
        defaultImage = indicatorImage.sprite;
        defaultText = titleText.text;
    }

    /// <summary>
    /// Sets the image color for the indicator.
    /// </summary>
    /// <param name="color"></param>
    public void SetImageColor(Color color)
    {
        indicatorImage.color = color;
        titleText.color = color;
    }
    
    public void SetText(string text)
    {
        titleText.text = text != null ? text : defaultText;
    }
    
    public void SetImage(Sprite image)
    {
        indicatorImage.sprite = image != null ? image : defaultImage;
    }

    /// <summary>
    /// Sets the distance text for the indicator.
    /// </summary>
    /// <param name="value"></param>
    public void SetDistanceText(float value)
    {
        distanceText.text = value >= 0 ? Mathf.Floor(value) + " m" : "";
    }

    /// <summary>
    /// Sets the distance text rotation of the indicator.
    /// </summary>
    /// <param name="rotation"></param>
    public void SetTextRotation(Quaternion rotation)
    {
        distanceText.rectTransform.rotation = rotation;
    }

    /// <summary>
    /// Sets the indicator as active or inactive.
    /// </summary>
    /// <param name="value"></param>
    public void Activate(bool value, GameObject go = null)
    {
        if (go == null) {
            Debug.Log($"Setting unknown to: {value}");
        }
        Debug.Log($"Setting inidcator for {go}: {value}");
        transform.gameObject.SetActive(value);
    }

    void OnDisable() {
        Debug.Log($"{gameObject} got disabled");
    }
}

public enum IndicatorType
{
    BOX,
    ARROW
}
