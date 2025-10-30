using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    public float MaxForce;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(Random.Range(0, MaxForce),0,0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
