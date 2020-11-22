using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpaceMap;

//[CustomEditor(typeof(PlanetNode))]
//public class PlanetEditor : Editor
//{
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    void OnSceneGUI()
//    {
//        PlanetNode handleExample = (PlanetNode)target;
//        if (handleExample == null)
//        {
//            return;
//        }

//        //Handles.color = Color.yellow;
//        //Handles.Label(handleExample.transform.position, handleExample.gameObject.name);

//        //Handles.BeginGUI();
//        //if (GUILayout.Button("Reset Area", GUILayout.Width(100)))
//        //{
//        //    //handleExample.shieldArea = 5;
//        //}
//        //Handles.EndGUI();

//    }


//}

public class PlanetNodeGizmoDrawer
{
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    public static void DrawGizmoForMyScript(PlanetNode planetNode, GizmoType gizmoType)
    {
        Handles.color = Color.yellow;
        Handles.Label(planetNode.transform.position, planetNode.gameObject.name);
    }
}
