using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class FileHandler : MonoBehaviour
{
    public FileScriptableObject file;
    public SpriteRenderer fileIcon;
    public TMP_Text fileName;
    public Rigidbody2D rb;
    public float pipeSpeed;
    public float pipeSize;
    public Transform bottomPoint;
    public string IP;
    public TooltipHandler tooltip;
    public float tooltipOffset = 1f;
    
    public WeightedList<ScriptableObject> files;
    
    public Vector2 moveDirection { get; set; }
    public bool isEntered = false;
    private Vector2 lastPosition;

    public bool isCought;
    public PlayerHand coughtHand;
    
    [Header("Pop Tween")]
    public float popDuration = 0.25f;
    public float popOvershoot = 1.2f;

    [Header("Random Physics")]
    public float minImpulse = 6f;
    public float maxImpulse = 12f;
    public float minTorque = 60f;
    public float maxTorque = 180f;

    public event UnityAction<FileHandler> OnConsumed;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (bottomPoint != null)
        {
            // Rigidbody2D.centerOfMass is in local space:
            rb.centerOfMass = bottomPoint.localPosition;
        }
    }

    public void SetupForFileSettings(FileScriptableObject settings)
    {
        file = settings;
        fileName.text = file.fileName;
        fileName.gameObject.SetActive(false);
        fileIcon.sprite = file.fileIcon;
        rb.bodyType = RigidbodyType2D.Kinematic;
        transform.localScale = Vector3.one * pipeSize;

        Func<byte> randomIP = () => (byte)Random.Range(0, 256);

        IP = $"{randomIP()}.{randomIP()}.{randomIP()}.{randomIP()}";
    }

    public void SetupForRandomFile()
    {
        SetupForFileSettings(files.ChooseRandom() as FileScriptableObject);
    }

    // Update is called once per frame
    void Update()
    {
        lastPosition = transform.position;
        if (!isEntered)
            transform.position += (Vector3)moveDirection * (Time.deltaTime * pipeSpeed);
    }
    
    void LateUpdate()
    {
        if (tooltip)
        {
            tooltip.transform.position = transform.position + Vector3.up * tooltipOffset;
            tooltip.transform.rotation = Quaternion.identity;
        }
    }


    private void OnMouseEnter()
    {
        tooltip.ShowTooltip();
    }

    private void OnMouseExit()
    {
        tooltip.HideTooltip();
    }

    public void OnEntered(bool applyForce = true)
    {
        if (isEntered) return;
        isEntered = true;

        TaskManager.Get().AddFile(this);

        rb.bodyType = RigidbodyType2D.Dynamic;
        
        fileName.gameObject.SetActive(true);

        // Pop back to Vector.one with DOTween
        transform.DOKill(); // ensure no conflicting tweens
        transform.DOScale(Vector3.one, popDuration)
            .SetEase(Ease.OutBack, popOvershoot);

        if (applyForce)
        {
            // Apply random impulse force
            var impulse = Random.Range(minImpulse, maxImpulse);
            var forceDir = ((Vector2)transform.position - lastPosition).normalized;
            rb.AddForce(forceDir * impulse, ForceMode2D.Impulse);

            // Apply random spin (torque), random direction
            var torque = Random.Range(minTorque, maxTorque) * (Random.value < 0.5f ? -1f : 1f);
            rb.AddTorque(torque);
        }
    }

    public void OnCought(PlayerHand hand)
    {
        coughtHand = hand;
        isCought = true;
    }

    public void OnReleased()
    {
        coughtHand = null;
        isCought = false;
    }

    private void OnDestroy()
    {
        OnConsumed?.Invoke(this);
    }
}
