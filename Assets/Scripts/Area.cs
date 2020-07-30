using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private static float L;
    private static float R;
    private static float U;
    private static float D;
    public Vector2 center;
    public float width;
    public float height;

    public static bool IsInside(Vector2 point)
    {
        if (point.x < L) return false;
        if (point.x > R) return false;
        if (point.y > U) return false;
        if (point.y < D) return false;
        return true;
    }

    public static bool IsNotInside(Vector2 point)
    {
        return !IsInside(point);
    }

    // Start is called before the first frame update
    void Start()
    {
        L = center.x - width / 2;
        R = center.x + width / 2;
        U = center.y + height / 2;
        D = center.y - height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        //Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
        Gizmos.DrawWireCube(center, new Vector3(width, height, 1));
    }
}

//public class AreaGizmoDrawer
//{
//    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
//    public static void DrawGizmoForMyScript(PlanetNode planetNode, GizmoType gizmoType)
//    {
//        Handles.Label(planetNode.transform.position, planetNode.gameObject.name);
//    }
//}
