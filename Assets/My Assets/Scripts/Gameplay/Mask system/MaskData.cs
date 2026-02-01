using UnityEngine;

[CreateAssetMenu(fileName = "NewMask", menuName = "System/Mask Data")]
public class MaskData : ScriptableObject
{
    public string maskName;
    public Sprite icon;
    public float playerSpeedMultiplier = 1f;
    public Color environmentColor = Color.white;
}