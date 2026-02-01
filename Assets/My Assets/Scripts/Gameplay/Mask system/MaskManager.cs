using UnityEngine;
using System.Collections.Generic;

public class MaskManager : MonoBehaviour
{
    [Header("Setup")]
    public List<MaskData> maskList;
    [Tooltip("The object that rotates (the icons ring). Assign SlotsPivot here. Must have Pivot X:0.5, Y:0.5.")]
    public RectTransform pivotTransform;
    [Tooltip("Same as Pivot Transform: the empty that rotates (SlotsPivot). Leave empty to use first child of Center Square.")]
    public RectTransform slotsPivot;
    [Tooltip("The center square that stays fixed (e.g. CenterStatic / 'No mask'). Never rotates.")]
    public RectTransform centerSquare;
    [Header("Settings")]
    public float smoothSpeed = 10f;
    [Tooltip("If true, positions the rotating transform's children in a circle. Ensures equal distances when rotating.")]
    public bool layoutChildrenInCircle = true;
    [Tooltip("Radius of the circle in pixels (try 150–200).")]
    public float circleRadius = 150f;

    private int currentIndex = 0;
    private float targetAngle = 0f;

    public static MaskData ActiveMask;

    void Start()
    {
        RectTransform rotating = GetRotatingTransform();
        if (rotating != null)
        {
            SetPivotToCenter(rotating);
            if (layoutChildrenInCircle && maskList != null && maskList.Count > 0)
                LayoutChildrenInCircle(rotating);
        }
        if (maskList != null && maskList.Count > 0)
        {
            targetAngle = GetAngleForIndex(currentIndex);
            UpdateActiveMask();
        }
    }

    void LayoutChildrenInCircle(RectTransform rotatingTransform)
    {
        int count = Mathf.Min(rotatingTransform.childCount, maskList != null ? maskList.Count : rotatingTransform.childCount);
        if (count == 0) return;

        float angleStep = 360f / count;
        // Offset 90° so the first icon appears at the top
        float offset = 90f;

        for (int i = 0; i < count; i++)
        {
            RectTransform child = rotatingTransform.GetChild(i) as RectTransform;
            if (child == null) continue;

            float angleDeg = (i * angleStep) + offset;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            child.anchoredPosition = new Vector2(Mathf.Cos(angleRad) * circleRadius, Mathf.Sin(angleRad) * circleRadius);
            child.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    void SetPivotToCenter(RectTransform rect)
    {
        if (rect == null) return;
        Vector2 size = rect.rect.size;
        Vector2 oldPivot = rect.pivot;
        Vector2 centerPivot = new Vector2(0.5f, 0.5f);
        if (oldPivot == centerPivot) return;
        rect.pivot = centerPivot;
        rect.anchoredPosition += new Vector2((centerPivot.x - oldPivot.x) * size.x, (centerPivot.y - oldPivot.y) * size.y);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) RotateWheel(1);
        if (Input.GetKeyDown(KeyCode.E)) RotateWheel(-1);

        if (maskList == null || maskList.Count == 0) return;

        RectTransform rotatingTransform = GetRotatingTransform();
        if (rotatingTransform != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            rotatingTransform.localRotation = Quaternion.Slerp(rotatingTransform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
            KeepChildrenUpright(rotatingTransform);
        }
        if (centerSquare != null)
        {
            if (centerSquare == pivotTransform || centerSquare == rotatingTransform)
                centerSquare.localRotation = Quaternion.identity;
            else if (centerSquare.parent != rotatingTransform)
                centerSquare.localRotation = Quaternion.identity;
        }
    }

    RectTransform GetRotatingTransform()
    {
        if (pivotTransform == null) return null;
        if (slotsPivot != null) return slotsPivot;
        if (centerSquare == pivotTransform && pivotTransform.childCount > 0)
        {
            Transform first = pivotTransform.GetChild(0);
            if (first is RectTransform rt) return rt;
        }
        return pivotTransform;
    }

    void KeepChildrenUpright(RectTransform rotatingTransform)
    {
        Quaternion inverseRotation = Quaternion.Inverse(rotatingTransform.localRotation);
        for (int i = 0; i < rotatingTransform.childCount; i++)
        {
            RectTransform child = rotatingTransform.GetChild(i) as RectTransform;
            if (child != null)
                child.localRotation = inverseRotation;
        }
    }

    void RotateWheel(int direction)
    {
        if (maskList == null || maskList.Count == 0)
        {
            Debug.LogWarning("[MaskManager] Q/E pressed but mask list is empty. Assign at least one MaskData in the Inspector.");
            return;
        }

        currentIndex = (currentIndex + direction + maskList.Count) % maskList.Count;
        targetAngle = GetAngleForIndex(currentIndex);

        UpdateActiveMask();
    }

    float GetAngleForIndex(int index)
    {
        if (maskList == null || maskList.Count == 0) return 0f;
        float angleStep = 360f / maskList.Count;
        return -index * angleStep;
    }

    void UpdateActiveMask()
    {
        if (maskList == null || maskList.Count == 0) return;

        ActiveMask = maskList[currentIndex];
        Debug.Log($"[MaskManager] Mask changed. Selected: {ActiveMask.maskName} (index {currentIndex})");
    }
}