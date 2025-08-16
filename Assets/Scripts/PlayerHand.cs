using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHand : MonoBehaviour
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

    private FileHandler pickedItem;
    private int handState = 0;

    private Sprite tipNormalSpr;

    private int originalLRSortingOrder;
    private int originalTipSortingOrder;

    private bool canDrag = true;

    public event UnityAction<PlayerHand> OnClicked;

    Coroutine currentGoTowards = null;

    public bool HasItem => pickedItem != null && handState >= 2;

    private void Start()
    {
        originalLRSortingOrder = lr.sortingOrder;
        originalTipSortingOrder = handTipSpr.sortingOrder;
        tipNormalSpr = handTipSpr.sprite;
        TaskManager.Get().OnGameOver += () => canDrag = false;
    }

    void MouseDown()
    {
        OnClicked?.Invoke(this);
    }

    public void PickupItem(FileHandler item)
    {
        if (pickedItem) return;

        pickedItem = item;

        handState = 1;

        RunGoTowards(pickedItem.transform, .5f, OnItemReached);
    }

    public void TransportItem(Vector3 position)
    {
        if (!pickedItem || handState == 3) return;

        handState = 3;

        RunGoTowards(position, .5f, OnItemTransported);
    }

    public void RunGoTowards(Vector3 target, float time, Action endCallback)
    {
        if (currentGoTowards != null) StopCoroutine(currentGoTowards);

        currentGoTowards = StartCoroutine(GoTowards(target, time, endCallback));
    }

    public void RunGoTowards(Transform target, float time, Action endCallback)
    {
        if (currentGoTowards != null) StopCoroutine(currentGoTowards);

        currentGoTowards = StartCoroutine(GoTowards(target, time, endCallback));
    }

    IEnumerator GoTowards(Vector3 target, float time, Action endCallback)
    {
        Vector3 startingPos = tip.transform.position;
        Vector3 endPos = target;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            tip.transform.position = Vector3.Lerp(startingPos, endPos, t / time);

            yield return null;
        }

        tip.transform.position = endPos;

        currentGoTowards = null;

        endCallback?.Invoke();
    }

    IEnumerator GoTowards(Transform target, float time, Action endCallback)
    {
        Vector3 startingPos = tip.transform.position;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            tip.transform.position = Vector3.Lerp(startingPos, target.position, t / time);

            yield return null;
        }

        tip.transform.position = target.position;

        currentGoTowards = null;

        endCallback?.Invoke();
    }

    void OnItemReached()
    {
        handState = 2;
     
        RunGoTowards(transform, pickedItem.file.size, null);
    }

    void OnItemTransported()
    {
        handState = 0;
        pickedItem = null;

        RunGoTowards(transform, .5f, null);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.IsPaused) return;

        MouseClickDetect();

        if (handState >= 2)
        {
            lr.sortingOrder = originalLRSortingOrder + zOrderSelectedAddition;
            handTipSpr.sortingOrder = originalTipSortingOrder + zOrderSelectedAddition;
        }
        else
        {
            lr.sortingOrder = originalLRSortingOrder;
            handTipSpr.sortingOrder = originalTipSortingOrder;
        }

        if (tipHoldSpr != null && pickedItem != null && handState != 1)
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

        if (handState >= 2 && pickedItem != null)
        {
            pickedItem.rb.position = tip.transform.position;
            pickedItem.rb.linearVelocity = Vector2.zero;
            pickedItem.rb.angularVelocity = 0;
        }
    }

    public void ForceUngrip()
    {
        if (pickedItem == null) return;

        handState = 0;
        pickedItem = null;

        RunGoTowards(transform, .5f, null);
    }

    public void OnSelected()
    {
        Debug.Log("meow :D");
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one * 0.25f, .5f);
             //.SetEase(Ease.InOutBack, 3f);
    } 

    public void OnDeselected()
    {

    } 
}
