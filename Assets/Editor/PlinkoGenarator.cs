using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System;
using Unity.VisualScripting;
using Unity.Mathematics;
using System.Collections.Generic;

public class PlinkoGen : EditorWindow
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    static int MaxDepth = 1;
    static float Down = 1;
    static float Horisontal = 1;

    static List<Vector3> Postions;

    [MenuItem("Tools/Gen Pins")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(PlinkoGen));
        window.Show();
    }

    void OnGUI()
    {
        MaxDepth = EditorGUILayout.IntField("Max Depth:", MaxDepth);
        Down = EditorGUILayout.FloatField("Down:", Down);
        Horisontal = EditorGUILayout.FloatField("Horisontal:", Horisontal);
        
        if (GUILayout.Button("Generate pins"))
        {
            GameObject pin = Selection.activeGameObject;
            if (pin == null)
            {
                Debug.Log("No object selected");
                return;
            }
            Postions = new();
            GenNewPin(0, pin);

        }
    }

    void GenNewPin(int Depth, GameObject pin)
    {
        Debug.Log($"{Depth} | {MaxDepth}");
        Debug.Log($"{Horisontal} | {Down}");
        if (Depth >= MaxDepth)
        {
            Debug.Log("F");
            return;
        }
        Vector3 pos = pin.transform.position;
        if (Postions.Contains(pos))
        {
            Debug.Log("E");
            return;
        }
        Postions.Add(pos);
        GenNewPin(Depth + 1, Instantiate(pin, new Vector3(pos.x + Horisontal, pos.y - Down, pos.z), pin.transform.rotation));
        GenNewPin(Depth + 1, Instantiate(pin, new Vector3(pos.x - Horisontal, pos.y - Down, pos.z), pin.transform.rotation));
        
        
    }
}
