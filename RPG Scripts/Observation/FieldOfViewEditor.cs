using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Observation
{

    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fow = (FieldOfView)target;
            if (fow.Eye == null) return;
            Handles.color = fow.Colour;
            Handles.DrawWireArc(fow.Eye.position, fow.Eye.up, fow.Eye.forward, 360, fow.ViewRadius);
            Vector3 viewAngleA =  fow.DirectionFromAngle(-fow.ViewAngle / 2, true);
            Vector3 viewAngleB =  fow.DirectionFromAngle(fow.ViewAngle / 2, true);
            Handles.DrawLine(fow.Eye.position, fow.Eye.position + viewAngleA * fow.ViewRadius);
            Handles.DrawLine(fow.Eye.position, fow.Eye.position + viewAngleB * fow.ViewRadius);

            //foreach (GameObject visableTarget in fow.VisibleTargets)
            //{
            //    Handles.DrawLine(fow.Eye.position, visableTarget.transform.position);
            //}
        }
    }
}
