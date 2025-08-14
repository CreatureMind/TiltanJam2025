using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform tip;

    [SerializeField, ReadOnly]
    private bool isDragging = false;

    private FileHandler file;

    void OnMouseDrag()
    {
        isDragging = true;
    }

    void OnMouseUp()
    {
        this.file = CheckForHit();

        if (file == null)
            isDragging = false;
        else
        {
            
        }
    }

    FileHandler CheckForHit()
    {
        if (!isDragging) return null;

        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit == null || hit.collider == null || !hit.collider.TryGetComponent(out FileHandler file)) return null;

        return file;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDragging) GoToOrigin();
        else FollowMouse();

        lr.positionCount = 2;
        lr.SetPositions(new Vector3[]{
            origin.position,
            tip.position
        });

        var directionVector = tip.position - origin.position;

        var textScale = lr.textureScale;
        textScale.y = directionVector.x > 0 ? -1 : 1;
        lr.textureScale = textScale;

        tip.rotation = Quaternion.LookRotation(Vector3.forward, directionVector);
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
}
