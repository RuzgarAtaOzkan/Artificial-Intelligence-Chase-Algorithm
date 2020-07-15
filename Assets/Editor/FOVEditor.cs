using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FOVEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target as FieldOfView;
        Handles.color = Color.red;

        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewAngle);

        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        foreach (Transform visibleTarget in fov.visibleTargets)
        {
            Debug.DrawLine(fov.transform.position, visibleTarget.position, Color.blue);
        }
    }
}
