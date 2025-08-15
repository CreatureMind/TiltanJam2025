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
    public float pipeSpeed;
    
    public WeightedList<ScriptableObject> files;
    
    public Vector2 moveDirection { get; set; }
    private bool isEntered = false;
    private Vector2 lastPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        file = files.ChooseRandom() as FileScriptableObject;
        fileName.text = file.fileName;
        fileIcon.sprite = file.fileIcon;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.localScale = Vector3.one * 0.5f;
    }
    
    // Update is called once per frame
    void Update()
    {
        lastPosition = transform.position;
        if (!isEntered)
            transform.position += (Vector3)moveDirection * (Time.deltaTime * pipeSpeed);
    }
    
    public void OnEntered()
    {
        isEntered = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        transform.localScale = Vector3.one;
        var forceDir = ((Vector2)transform.position - lastPosition).normalized;
        rb.AddForce(forceDir * 10, ForceMode2D.Impulse);
        rb.AddTorque(100);
    }
}
