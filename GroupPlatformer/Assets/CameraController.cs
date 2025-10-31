using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<CameraPointNode> points;
    private void Update()
    {
        int closest = -1;
        Vector2 playerPos = (Vector2)PlayerMovement.main.transform.position;
        for (int i = 0; i < points.Count; i++)
            if (points[i].bounds.Contains(playerPos) &&(closest==-1||(playerPos - points[i].pos).magnitude< (playerPos - points[closest].pos).magnitude))
                closest = i;
        Vector2 camTarget = playerPos;
        float camTargetSize = PlayerMovement.main.velocity.magnitude*.01f+4f;
        if (closest!=-1)
        {
            camTarget = points[closest].pos + (playerPos- points[closest].pos)*.05f;
            camTargetSize = points[closest].OrthSize;
        }
        Camera.main.transform.position = Vector3.Lerp((Vector3)camTarget - Vector3.forward * 10,Camera.main.transform.position,Mathf.Pow(0.1f,Time.deltaTime));
        Camera.main.transform.position = (Vector3)Vector2.Scale(Vector2.ClampMagnitude(Vector2.Scale((Vector2)(Camera.main.transform.position-PlayerMovement.main.transform.position),new Vector2((float)Screen.height/Screen.width,1f)),Camera.main.orthographicSize-.25f), new Vector2((float)Screen.width / Screen.height, 1f)) + PlayerMovement.main.transform.position - Vector3.forward * 10;
        Camera.main.orthographicSize = Mathf.Lerp(camTargetSize,Camera.main.orthographicSize,Mathf.Pow(0.05f,Time.deltaTime));
    }
}