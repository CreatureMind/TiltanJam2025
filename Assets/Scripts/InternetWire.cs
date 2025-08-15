using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InternetWire : MonoBehaviour, IHitReciever
{
    [SerializeField] private InternetWireScriptableObject settings;
    [SerializeField] private Rigidbody2D bottomRb;
    public List<string> requiredIPs;
    [SerializeField] private Transform wireTip;
    [SerializeField] private FileScriptableObject corruptFile;
    [SerializeField] private FileHandler fileSpawnPrefab;

    [SerializeField] private List<SpriteRenderer> visuals;
    [SerializeField] private SpriteRenderer tipVisual;

    private bool isLeaving;
    private bool isCorrupted;

    Coroutine waitRoutiene = null;

    public event UnityAction<InternetWire> OnWireComplete;

    public void SetupWithSettings(InternetWireScriptableObject settings)
    {
        this.settings = settings;
        UpdateVisuals();
    }

    private void Start()
    {
        if (settings)
            SetupWithSettings(settings);

        StartCoroutine(WaitToStart());
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(Random.Range(settings.spawnDelayTime.x, settings.spawnDelayTime.y));

        WireEnter();

        StartCoroutine(WaitToLeave());
    }

    IEnumerator WaitToLeave()
    {
        yield return new WaitForSeconds(Random.Range(settings.stayTime.x, settings.stayTime.y));

        WireLeave();
    }

    void UpdateVisuals()
    {
        bool isOnBack = Random.value > 0.5f;

        UnityAction<SpriteRenderer, int> modifyVisuals = (renderer, ZOffset) =>
        {
            renderer.sortingOrder = settings.zOrder * (isOnBack ? -1 : 1) + ZOffset;
            if (isOnBack)
                transform.localScale = Vector3.one * 0.7f;
            renderer.color = settings.color;
            if (isOnBack)
                renderer.color -= new Color32(100, 100, 100, 0);
        };

        foreach (var item in visuals)
        {
            modifyVisuals(item, 0);
        }

        modifyVisuals(tipVisual, 1);
    }

    public void hitRecieved(int hitID, IHitReciever.HitType type, bool isTriggerHit, GameObject other)
    {
        if (isLeaving || isCorrupted) return;

        if (hitID == 0 & type == IHitReciever.HitType.Enter && other.TryGetComponent(out FileHandler file))
        {
            if (file.file.isVirus)
            {
                MakeCorrupt();
                OnValidIPFound(file);

                TaskManager.Get().WriteToConsole($"end user req pong result: all is good!");
            }
            else if (requiredIPs.Contains(file.IP))
            {
                requiredIPs.Remove(file.IP);
                OnValidIPFound(file);

                TaskManager.Get().WriteToConsole($"end user req pong result: 0");
            }
            else
            {
                WireLeave();

                TaskManager.Get().WriteToConsole($"end user req pong result: -1");
            }
        }
    }

    void OnValidIPFound(FileHandler file)
    {
        if (file.coughtHand != null)
            file.coughtHand.ForceUngrip();

        file.transform.DOKill(); // ensure no conflicting tweens
        file.transform.DOScale(Vector3.zero, .5f)
            .SetEase(Ease.InBack, 1.2f);
        file.transform.DOMove(wireTip.position, .5f)
            .SetEase(Ease.InOutExpo, 2);

        file.GetComponent<Collider2D>().enabled = false;

        Wiggle();

        Destroy(file.gameObject, .5f);

        if (requiredIPs.Count == 0 && !isCorrupted)
        {
            WireLeave();
        }
    }

    void MakeCorrupt()
    {
        if (isCorrupted) return;
        isCorrupted = true;

        StartCoroutine(CrazyWiggle());

        Invoke("SpawnCorruptFile", .5f);
        Invoke("SpawnCorruptFile", 1.5f);

        Invoke("WireLeave", 2);
    }

    IEnumerator CrazyWiggle()
    {
        float wiggles = 5;
        for (int i = 0; i < wiggles; i++)
        {
            bottomRb.linearVelocity = Vector2.zero;
            bottomRb.AddForce(new Vector2(7.5f, 0) * (i % 2 == 0 ? -1 : 1), ForceMode2D.Impulse);
            yield return new WaitForSeconds(2.0f / wiggles);
        }
    }

    public void SpawnCorruptFile()
    {
        var fileNew = Instantiate(fileSpawnPrefab, bottomRb.transform.position, Quaternion.identity);
        fileNew.SetupForFileSettings(corruptFile);
        fileNew.OnEntered(false);
        Wiggle();
    }

    void WireLeave()
    {
        if (isLeaving) return;
        isLeaving = true;

        OnWireComplete?.Invoke(this);

        transform.DOKill();
        transform.DOMove(transform.position + (Vector3.up * 4), 1)
            .SetEase(Ease.InBack, .5f);

        Destroy(gameObject, 1);
    }

    void WireEnter()
    {
        Wiggle();
        transform.DOKill();
        transform.DOMove(transform.position - (Vector3.up * Random.Range(3.0f, 4.0f)), 1)
            .SetEase(Ease.OutBack, .5f);

        if (requiredIPs.Count != 0)
            TaskManager.Get().WriteToConsole($"end user req packet of IP: {requiredIPs[0]}");
    }

    void Wiggle()
    {
        bottomRb.AddForce(new Vector2(Random.Range(2, 7.5f) * (Random.value > .5f ? -1 : 1), 0), ForceMode2D.Impulse);
    }
}
