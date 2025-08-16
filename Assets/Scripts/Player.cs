using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerHand[] hands;
    [SerializeField, ReadOnly] private PlayerHand selectedHand;

    private bool selectedThisFrame = false;

    private void Start()
    {
        foreach (var item in hands)
        {
            item.OnClicked += HandClicked;
        }
    }

    void HandClicked(PlayerHand hand)
    {
        if (selectedHand != hand)
            selectedThisFrame = true;

        if (selectedHand != null)
            selectedHand?.OnDeselected();
        selectedHand = hand;
        if (selectedHand != null)
            selectedHand.OnSelected();
    }

    private void LateUpdate()
    {
        if (selectedThisFrame)
        {
            selectedThisFrame = false;
            return;
        }

        if (Input.GetMouseButtonDown(0) && selectedHand != null)
        {
            if (selectedHand.HasItem)
            {
                selectedHand.TransportItem(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else
            {
                List<RaycastHit2D> files = new();

                Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, new ContactFilter2D().NoFilter(), files);
                var filesList = files.Where(e => e.transform.TryGetComponent(out FileHandler _)).Select(e => e.transform.GetComponent<FileHandler>()).ToList();

                if (filesList.Count != 0 && filesList[0].isEntered)
                    selectedHand.PickupItem(filesList[0]);
            }
        }
    }
}
