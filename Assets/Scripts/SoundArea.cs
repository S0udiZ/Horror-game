using System;
using UnityEngine;

public class SoundArea : MonoBehaviour
{
    public int Priority;
    public Enemy Enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Enemy == null)
        {
            Debug.LogError("Missing enemy");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Enemy.SendSound(transform.position, Priority);
        Debug.Log("Send sound");
    }
}
