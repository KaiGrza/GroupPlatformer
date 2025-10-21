using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class CameraPointNode : MonoBehaviour
{
    public CameraController controller;
    public Vector2 pos;
    public float OrthSize;
    public Rect bounds;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1f, 0.3f,.04f);
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
}
[CustomEditor(typeof(CameraPointNode))]
public class CameraPointNodeEditor : Editor
{
    // Custom in-scene UI for when ExampleScript
    // component is selected.
    public void OnSceneGUI()
    {
        var t = target as CameraPointNode;
        Handles.color = Color.yellow;
        t.pos = (Vector2)Handles.FreeMoveHandle((Vector3)t.pos, .5f, Vector3.zero, Handles.CubeHandleCap);
        t.OrthSize = Handles.FreeMoveHandle((Vector3)t.pos + Vector3.up * t.OrthSize, .25f, Vector3.zero, Handles.CircleHandleCap).y - t.pos.y;
        Handles.color = Color.white;
        Handles.DrawWireCube((Vector3)t.pos, new Vector3(16f / 9f, 1f, .1f) * 2 * t.OrthSize);
        Handles.color = t.bounds.width<=0||t.bounds.height<=0?Color.red:Color.green;
        Handles.DrawSolidRectangleWithOutline(t.bounds, t.bounds.width <= 0 || t.bounds.height <= 0 ? new Color(1, 0f, 0f, 0.1f): new Color(0, 1f, .3f, 0.1f), Handles.color);
        t.bounds.min = (Vector2)Handles.FreeMoveHandle(new Vector3(t.bounds.xMin, t.bounds.yMin, 0), .25f, Vector3.zero, Handles.CubeHandleCap);
        t.bounds.max = (Vector2)Handles.FreeMoveHandle(new Vector3(t.bounds.xMax, t.bounds.yMax, 0), .25f, Vector3.zero, Handles.CubeHandleCap);
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Add new point"))
        {
            if((target as CameraPointNode).controller==null)
            {
                Debug.LogWarning("Controller not set for this Point Node.");
                return;
            }

            if ((target as CameraPointNode).controller.points == null)
            {
                Debug.LogWarning("Controller does not have an initialized list");
                return;
            }
            CameraPointNode cpn = Instantiate(target as CameraPointNode,(target as CameraPointNode).controller.transform);
            cpn.name = "CameraPoint";
            (target as CameraPointNode).controller.points.Add(cpn);
            Selection.activeObject = cpn;
        }
    }
}