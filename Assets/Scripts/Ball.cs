using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    public float MaxForce;
    public bool reset = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(new Vector3(Random.Range(-MaxForce, MaxForce), 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            transform.position = new Vector3(0, 4.5f, 1.5f);
            rb.linearVelocity = new Vector3(0, 0, 0);
            reset = false;
            Start();
        }
    }
}
