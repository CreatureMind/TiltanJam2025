using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InternetWire : MonoBehaviour, IHitReciever
{
    [SerializeField] private Rigidbody2D bottomRb;
    [SerializeField] private List<string> requiredIPs;
    [SerializeField] private Transform wireTip;
    [SerializeField] private FileScriptableObject corruptFile;
    [SerializeField] private FileHandler fileSpawnPrefab;
    private bool isLeaving;
    private bool isCorrupted;

    public event UnityAction<InternetWire> OnWireComplete;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            bottomRb.AddForce(new Vector2(10, 0), ForceMode2D.Impulse);
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
            }
            else if (requiredIPs.Contains(file.IP))
            {
                requiredIPs.Remove(file.IP);
                OnValidIPFound(file);
            }
            else
            {
                WireLeave();
            }
        }
    }

    void OnValidIPFound(FileHandler file)
    {
        if (file.coughtHand != null)
            file.coughtHand.ForceUngrip();

        file.isEntered = false;
        file.transform.DOKill(); // ensure no conflicting tweens
        file.transform.DOScale(Vector3.zero, .5f)
            .SetEase(Ease.InBack, 1.2f);
        file.transform.DOMove(wireTip.position, .5f)
            .SetEase(Ease.InOutExpo, 2);

        Destroy(file.gameObject, .5f);

        if (requiredIPs.Count == 0 && !isCorrupted)
        {
            OnWireComplete?.Invoke(this);

            WireLeave();
        }
    }

    void MakeCorrupt()
    {
        if (isCorrupted) return;
        isCorrupted = true;

        Invoke("SpawnCorruptFile", .5f);
        Invoke("SpawnCorruptFile", 1.5f);

        Invoke("WireLeave", 2);
    }

    public void SpawnCorruptFile()
    {
        var fileNew = Instantiate(fileSpawnPrefab, transform.position, Quaternion.identity);
        fileNew.SetupForFileSettings(corruptFile);
        fileNew.OnEntered(false);
    }

    void WireLeave()
    {
        if (isLeaving) return;
        isLeaving = true;

        transform.DOKill();
        transform.DOMove(transform.position + (Vector3.up * 4), 1)
            .SetEase(Ease.InBack, .5f);

        Destroy(gameObject, 1);
    }
}
