using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FileScriptableObject", menuName = "Scriptable Objects/FileScriptableObject")]
public class FileScriptableObject : ScriptableObject
{
    public string fileName;
    public Sprite fileIcon;
    public bool isImage;
    public float size = 1;
    [ShowIf("isImage")] 
    public Sprite picture;
    public bool isVirus;
    public bool isFolder;
    [ShowIf("isFolder")]
    public ScriptableObject[] files;
    public bool isLocked;
    [ShowIf("isLocked")]
    public string password;
}
