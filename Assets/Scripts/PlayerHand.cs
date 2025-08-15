using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHand : MonoBehaviour, IHitReciever
{
    [BoxGroup("References")]
    [SerializeField] private LineRenderer lr;
    [BoxGroup("References")]
    [SerializeField] private Transform origin;
    [BoxGroup("References")]
    [SerializeField] private Transform tip;
    [BoxGroup("References")]
    [SerializeField] private SpriteRenderer handTipSpr;

    [BoxGroup("Options")]
    [SerializeField] private int zOrderSelectedAddition;
    [BoxGroup("Options")]
    [SerializeField] private Sprite tipHoldSpr;
    [BoxGroup("Options")]
    [SerializeField] private Vector2 originOffset;

    private Sprite tipNormalSpr;

    private Vector3? targetPos = null;
    private int originalLRSortingOrder;
    private int originalTipSortingOrder;

    [SerializeField, ReadOnly]
    private bool isDragging = false;

    private FileHandler file = null;

    private void Start()
    {
        originalLRSortingOrder = lr.sortingOrder;
        originalTipSortingOrder = handTipSpr.sortingOrder;
        tipNormalSpr = handTipSpr.sprite;
    }

    void MouseUp()
    {
        if (file != null && !targetPos.HasValue)
        {
            var newTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newTarget.z = 0;
            newTarget = newTarget.Clamp(new Vector3(-9, -5, 0), new Vector3(9, 5, 0));
            targetPos = newTarget;
            StartCoroutine(GoToTagetPos());
        }

        if (file == null)
            isDragging = false;
    }

    void MouseDown()
    {
        if (targetPos.HasValue) return;

        if (file == null)
            isDragging = true;
    }

    IEnumerator GoToTagetPos()
    {
        if (!targetPos.HasValue || file == null) yield break;

        float totalTime = 1 / file.file.size;
        Vector3 startingPos = tip.transform.position;
        Vector3 endPos = targetPos.Value;

        for (float t = 0; t < totalTime; t += Time.deltaTime)
        {
            tip.transform.position = Vector3.Lerp(startingPos, endPos, t / totalTime);

            yield return null;
        }

        tip.transform.position = endPos;

        ForceUngrip();
    }

    void MouseClickDetect()
    {
        Func<PlayerHand> raycastLambda = () =>
        {
            List<RaycastHit2D> outList = new();
            Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, new ContactFilter2D().NoFilter(), outList);

            outList = outList.Where(e =>
            {
                if (e.transform.parent != null && e.transform.parent.TryGetComponent(out PlayerHand hand) && hand == this)
                    return true;

                return false;
            }).ToList();

            if (outList.Count == 0) return null;

            return outList[0].transform.parent.GetComponent<PlayerHand>();
        };

        if (Input.GetMouseButtonDown(0))
        {
            var res = raycastLambda();
            if (res != null)
                res.MouseDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (file != null || isDragging)
                MouseUp();
        }
    }

    // Update is called once per frame
    void Update()
    {
        MouseClickDetect();

        if (!isDragging && this.file == null) GoToOrigin();
        else if (this.file == null) FollowMouse();

        if (this.file != null || isDragging)
        {
            lr.sortingOrder = originalLRSortingOrder + zOrderSelectedAddition;
            handTipSpr.sortingOrder = originalTipSortingOrder + zOrderSelectedAddition;
        }
        else
        {
            lr.sortingOrder = originalLRSortingOrder;
            handTipSpr.sortingOrder = originalTipSortingOrder;
        }

        if (tipHoldSpr != null && file != null)
            handTipSpr.sprite = tipHoldSpr;
        else
            handTipSpr.sprite = tipNormalSpr;

        var realOrigin = origin.position + (Vector3)originOffset;

        lr.positionCount = 2;
        lr.SetPositions(new Vector3[]{
            realOrigin,
            tip.position
        });

        var directionVector = tip.position - realOrigin;

        var textScale = lr.textureScale;
        textScale.y = directionVector.x > 0 ? -1 : 1;
        lr.textureScale = textScale;

        tip.rotation = Quaternion.LookRotation(Vector3.forward, directionVector);

        if (file != null)
            file.rb.position = (Vector2)tip.position;
    }

    void GoToOrigin()
    {
        tip.position = Vector3.Lerp(tip.position, transform.position, Time.deltaTime * 5);
    }

    void FollowMouse()
    {
        var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        tip.position = newPos;
    }

    public void hitRecieved(int hitID, IHitReciever.HitType type, bool isTriggerHit, GameObject other)
    {
        if (hitID == 0 && type == IHitReciever.HitType.Enter)
        {
            if (!isDragging || this.file != null || !other.TryGetComponent(out FileHandler file) || !file.isEntered || file.isCought) return;

            this.file = file;
            file.rb.bodyType = RigidbodyType2D.Kinematic;
            file.rb.linearVelocity = Vector2.zero;
            file.rb.angularVelocity = 0;
            file.OnCought(this);
            //on picked
        }
    }

    public void ForceUngrip()
    {
        if (file == null) return;

        isDragging = false;
        file.rb.bodyType = RigidbodyType2D.Dynamic;
        file.rb.linearVelocity = Vector2.zero;
        file.rb.angularVelocity = 0;
        file.OnReleased();
        file = null;
        targetPos = null;
    }
}
