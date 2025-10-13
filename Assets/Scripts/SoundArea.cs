using System;
using UnityEngine;

public class SoundArea : MonoBehaviour
{
    public Enemy enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (enemy == null)
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
        enemy.SendSound(transform.position, 3);
        Debug.Log("Send sound");
    }
}
