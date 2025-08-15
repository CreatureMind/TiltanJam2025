using System.Collections.Generic;
using UnityEngine;

public class InternetWire : MonoBehaviour, IHitReciever
{
    [SerializeField] private Rigidbody2D bottomRb;
    [SerializeField] private List<string> requiredIPs;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            bottomRb.AddForce(new Vector2(10, 0), ForceMode2D.Impulse);
    }

    public void hitRecieved(int hitID, IHitReciever.HitType type, bool isTriggerHit, GameObject other)
    {
        if (hitID == 0 & type == IHitReciever.HitType.Enter && other.TryGetComponent(out FileHandler file))
        {
            if (file.file.isVirus)
            {
                //AAAAAAAAAA VIRUS YARGGGG
            }
            else if (requiredIPs.Contains(file.IP))
            {
                requiredIPs.Remove(file.IP);
                OnValidIPFound();
            }
        }
    }

    void OnValidIPFound()
    {
        if (requiredIPs.Count == 0)
        {
            //yay wire complete! :D
        }
    }
}
