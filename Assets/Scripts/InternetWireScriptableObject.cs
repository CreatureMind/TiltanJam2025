using UnityEngine;

[CreateAssetMenu(fileName = "InternetWireScriptableObject", menuName = "Scriptable Objects/InternetWireScriptableObject")]
public class InternetWireScriptableObject : ScriptableObject
{
    public Color color = Color.white;
    public int zOrder;
    public Vector2 spawnDelayTime;
    public Vector2 stayTime;
}
