using UnityEngine;

[CreateAssetMenu(fileName = "newMask", menuName = "System/mask")]
public class MascaraData : ScriptableObject
{
    public string name;
    public Sprite icon;
    public float velocidadJugador = 5f;
    public Color colorAmbiente; // 
    public int dañoExtra = 0;
}