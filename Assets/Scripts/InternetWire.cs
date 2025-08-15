using UnityEngine;

public class InternetWire : MonoBehaviour
{
    [SerializeField] private Rigidbody2D bottomRb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            bottomRb.AddForce(new Vector2(10, 0), ForceMode2D.Impulse);
    }
}
