using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    private void Start()
    {
        guys = new List<Transform>();
        while(transform.childCount>0)
        {
            guys.Add(transform.GetChild(0));
            transform.GetChild(0).SetParent(Camera.main.transform, true);
        }
    }
    List<Transform> guys;
    private void Update()
    {
        foreach(Transform t in guys)
        {
            t.localScale = Vector3.one*Camera.main.orthographicSize/4f;
            t.localPosition = Vector3.Scale((transform.position-Camera.main.transform.position)/t.position.z * (t.position.z>=100?0:1),Vector3.one-Vector3.forward)+Vector3.forward*(t.position.z-Camera.main.transform.position.z);
        }
    }
}
