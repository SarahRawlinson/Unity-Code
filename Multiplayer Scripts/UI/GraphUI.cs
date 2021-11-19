using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerRTS.UI
{
    public class GraphUI : Graphic
    {
        public float thickness = 10f;
        //https://docs.unity3d.com/2017.4/Documentation/ScriptReference/UI.Graphic.html
        //https://www.youtube.com/watch?v=--LB7URk60A&ab_channel=GameDevGuide
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            // corners of square
            vertex.position = new Vector3(0, 0);
            vh.AddVert(vertex);
            vertex.position = new Vector3(0, height);
            vh.AddVert(vertex);
            vertex.position = new Vector3(width, height);
            vh.AddVert(vertex);
            vertex.position = new Vector3(width, 0);
            vh.AddVert(vertex);
            // triangles
            vh.AddTriangle(0,1,2);
            vh.AddTriangle(2,3,0);
        
            float widthSqr = thickness * thickness;
            float distanceSqr = widthSqr / widthSqr / 2f;
            float distance = Mathf.Sqrt(distanceSqr);
        
            vertex.position = new Vector3(distance, distance);
            vh.AddVert(vertex);
            vertex.position = new Vector3(distance, height - distance);
            vh.AddVert(vertex);
            vertex.position = new Vector3(width - distance, height - distance);
            vh.AddVert(vertex);
            vertex.position = new Vector3(width - distance, distance);
            vh.AddVert(vertex);
            
            // triangles left edge
            vh.AddTriangle(0,1,5);
            vh.AddTriangle(5,4,0);
            // triangles top edge
            vh.AddTriangle(1,2,6);
            vh.AddTriangle(6,5,1);
            // triangles right edge
            vh.AddTriangle(2,3,7);
            vh.AddTriangle(7,6,2);
            // triangles right edge
            vh.AddTriangle(3,0,4);
            vh.AddTriangle(4,7,3);
        }
    }
}
