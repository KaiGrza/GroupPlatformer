using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    private void Start()
    {
        guys = new List<Transform>();
        foreach (Transform t in transform)
        {
            guys.Add(t);
            t.SetParent(null, true);
        }
    }
    List<Transform> guys;
    private void Update()
    {
        foreach(Transform t in guys)
        {
            t.position = Vector3.Scale(Camera.main.transform.position+(transform.position-Camera.main.transform.position)/t.position.z * (t.position.z>=100?0:1),Vector3.one-Vector3.forward)+Vector3.forward*t.position.z;
        }
    }
}
