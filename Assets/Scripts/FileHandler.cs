using System.Collections.Generic;
using DG.Tweening;
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
    public bool isEntered = false;
    private Vector2 lastPosition;

    public bool isCought;
    
    [Header("Pop Tween")]
    public float popDuration = 0.25f;
    public float popOvershoot = 1.2f;

    [Header("Random Physics")]
    public float minImpulse = 6f;
    public float maxImpulse = 12f;
    public float minTorque = 60f;
    public float maxTorque = 180f;

    
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
        if (isEntered) return;
        isEntered = true;

        rb.bodyType = RigidbodyType2D.Dynamic;

        // Pop back to Vector.one with DOTween
        transform.DOKill(); // ensure no conflicting tweens
        transform.DOScale(Vector3.one, popDuration)
            .SetEase(Ease.OutBack, popOvershoot);

        // Apply random impulse force
        var impulse = Random.Range(minImpulse, maxImpulse);
        var forceDir = ((Vector2)transform.position - lastPosition).normalized;
        rb.AddForce(forceDir * impulse, ForceMode2D.Impulse);

        // Apply random spin (torque), random direction
        var torque = Random.Range(minTorque, maxTorque) * (Random.value < 0.5f ? -1f : 1f);
        rb.AddTorque(torque);
    }

    public void OnCought()
    {
        isCought = true;
    }

    public void OnReleased()
    {
        isCought = false;
    }
}
