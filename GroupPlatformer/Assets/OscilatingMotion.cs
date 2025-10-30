using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscilatingMotion : MonoBehaviour
{
    public Vector3 amplitude;
    public float frequency;
    Vector3 startPos;
    public void SetStartPos()
    {
        startPos = transform.localPosition;

    }
    private void Start()
    {
        SetStartPos();
    }
    private void Update()
    {
        transform.localPosition = startPos + amplitude * Mathf.Sin(Time.time * frequency * 2 * Mathf.PI);
    }
}
