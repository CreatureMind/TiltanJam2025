using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a interface to easily send hits/collisions from other objects to one specific object without creating a lot of small scripts
/// </summary>
public interface IHitReciever
{
    enum HitType
    {
        Enter,
        Stay,
        Exit
    }
    void hitRecieved(int hitID, HitType type, bool isTriggerHit, GameObject other);
}

public class hitSender : MonoBehaviour
{
    [SerializeField] private GameObject hitReciever;

    [Header("Hit data")]
    [SerializeField] private int hitID;
    [Space]
    [SerializeField] private bool getEnter;
    [SerializeField] private bool getStay;
    [SerializeField] private bool getExit;
    [Space]
    [SerializeField] private bool isTrigger;

    private IHitReciever hitRecieverInterface;

    private void Start()
    {
        hitRecieverInterface = hitReciever.GetComponent<IHitReciever>();

        if (hitRecieverInterface == null)
            throw new System.Exception($"Referenced hitReciever does not inherate 'IHitReciever' {gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitRecieverInterface == null || !getEnter || !isTrigger) return;

        hitRecieverInterface.hitRecieved(hitID, IHitReciever.HitType.Enter, isTrigger, collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hitRecieverInterface == null || !getStay || !isTrigger) return;

        hitRecieverInterface.hitRecieved(hitID, IHitReciever.HitType.Stay, isTrigger, collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hitRecieverInterface == null || !getExit || !isTrigger) return;

        hitRecieverInterface.hitRecieved(hitID, IHitReciever.HitType.Exit, isTrigger, collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hitRecieverInterface == null || !getEnter || isTrigger) return;

        hitRecieverInterface.hitRecieved(hitID, IHitReciever.HitType.Enter, isTrigger, collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (hitRecieverInterface == null || !getStay || isTrigger) return;

        hitRecieverInterface.hitRecieved(hitID, IHitReciever.HitType.Stay, isTrigger, collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (hitRecieverInterface == null || !getExit || isTrigger) return;

        hitRecieverInterface.hitRecieved(hitID, IHitReciever.HitType.Exit, isTrigger, collision.gameObject);
    }

    public int GetHitID() => hitID;
}
