using PixelPlay.OffScreenIndicator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Attach the script to the off screen indicator panel.
/// </summary>
[DefaultExecutionOrder(-1)]
public class OffScreenIndicator : MonoBehaviour
{
    [Range(0.5f, 0.9f)]
    [Tooltip("Distance offset of the indicators from the centre of the screen")]
    [SerializeField] private float screenBoundOffset = 0.9f;

    private Camera mainCamera;
    private Vector3 screenCentre;
    private Vector3 screenBounds;
    
    public Indicator arrowIndicator;
    public Indicator boxIndicator;

    private List<Target> targets = new List<Target>();

    public static Action<Target, bool> TargetStateChanged;

    void Start()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector3(Screen.width, Screen.height, 0) / 2;
        screenBounds = screenCentre * screenBoundOffset;
        TargetStateChanged += HandleTargetStateChanged;
    }

    public void SetCamera(Camera camera = null) {
        mainCamera = camera == null ? Camera.main : camera;
    }

    void LateUpdate()
    {
        DrawIndicators();
    }

    /// <summary>
    /// Draw the indicators on the screen and set thier position and rotation and other properties.
    /// </summary>
    void DrawIndicators()
    {
        foreach(Target target in targets)
        {
            Vector3 screenPosition = OffScreenIndicatorCore.GetScreenPosition(mainCamera, target.transform.position);
            bool isTargetVisible = OffScreenIndicatorCore.IsTargetVisible(screenPosition);
            float distanceFromCamera = target.NeedDistanceText ? target.GetDistanceFromCamera(mainCamera.transform.position) : float.MinValue;// Gets the target distance from the camera.
            Indicator indicator = null;

            if(target.NeedBoxIndicator && isTargetVisible)
            {
                screenPosition.z = 0;
                indicator = target.boxIndicator;
                target.arrowIndicator.Activate(false);
                indicator.SetImage(target.boxImage);
                indicator.SetText(target.boxText);
            }
            else if(target.NeedArrowIndicator && !isTargetVisible)
            {
                float angle = float.MinValue;
                OffScreenIndicatorCore.GetArrowIndicatorPositionAndAngle(ref screenPosition, ref angle, screenCentre, screenBounds);
                indicator = target.arrowIndicator;
                target.boxIndicator.Activate(false);
                indicator.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg); // Sets the rotation for the arrow indicator.
                indicator.SetImage(target.arrowImage);
                indicator.SetText(target.arrowText);
            } else {
                target.arrowIndicator.Activate(false);
                target.boxIndicator.Activate(false);
            }

            if (indicator != null) {
                indicator.SetImageColor(target.TargetColor);// Sets the image color of the indicator.
                indicator.SetDistanceText(distanceFromCamera); //Set the distance text for the indicator.
                indicator.transform.position = screenPosition; //Sets the position of the indicator on the screen.
                indicator.SetTextRotation(Quaternion.identity); // Sets the rotation of the distance text of the indicator.
                indicator.Activate(true, target.gameObject);
            }
        }
    }

    /// <summary>
    /// 1. Add the target to targets list if <paramref name="active"/> is true.
    /// 2. If <paramref name="active"/> is false deactivate the targets indicator, 
    ///     set its reference null and remove it from the targets list.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="active"></param>
    private void HandleTargetStateChanged(Target target, bool active)
    {
        if(active)
        {
            if (target.arrowIndicator == null) target.arrowIndicator = CreateIndicator(IndicatorType.ARROW);
            if (target.boxIndicator == null) target.boxIndicator = CreateIndicator(IndicatorType.BOX);
            targets.Add(target);
        }
        else
        {
            target.arrowIndicator.Activate(false);
            target.boxIndicator.Activate(false);
            targets.Remove(target);
        }
    }

    private Indicator CreateIndicator(IndicatorType type) {
        Indicator indicator = Instantiate(type == IndicatorType.BOX ? boxIndicator : arrowIndicator);
        indicator.transform.SetParent(transform, false);
        indicator.Activate(false);
        return indicator;
    }

    private void OnDestroy()
    {
        TargetStateChanged -= HandleTargetStateChanged;
    }
}
