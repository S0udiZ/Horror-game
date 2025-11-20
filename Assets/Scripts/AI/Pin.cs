using System;
using UnityEditor.PackageManager.Requests;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public Material Glow;
    public Ball controler;
    MeshRenderer rend;

    Material defaultMat;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        controler.Reset += OnReset;
        defaultMat = rend.material;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        rend.material = Glow;
    }

    void OnReset(object sender, EventArgs e)
    {
        rend.material = defaultMat;
    }
}
