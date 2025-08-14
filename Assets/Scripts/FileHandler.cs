using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FileHandler : MonoBehaviour
{
    public FileScriptableObject file;
    public SpriteRenderer fileIcon;
    public TMP_Text fileName;
    public Rigidbody2D rb;
    public List<ScriptableObject> files;
    
    private Vector2 lastPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fileName.text = file.fileName;
        fileIcon.sprite = file.fileIcon;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    
    // Update is called once per frame
    void Update()
    {
        lastPosition = transform.position;
    }
    
    public void OnEntered()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        var forceDir = ((Vector2)transform.position - lastPosition).normalized;
        rb.AddForce(forceDir * 1000);
        rb.AddTorque(100);
        
    }
}
