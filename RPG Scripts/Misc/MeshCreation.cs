using UnityEngine;
using System.Collections;

public class MeshCreation : MonoBehaviour
{

    // Use this for initialization
    private void Start()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[] 
        {
            new Vector3(-1, 1, -1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 0, -1),
            new Vector3(1, -0, 1),
            new Vector3(0, .5f, 0)
        };
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[15];

        //vertices[0] = new Vector3(-1, 1, -1);
        //vertices[1] = new Vector3(1, 1, 1);
        //vertices[2] = new Vector3(-1, 0, -1);
        //vertices[3] = new Vector3(1, -0, 1);
        //vertices[4] = new Vector3(0, .5f, 0);

        uv[0] = new Vector2(0, 1);
        uv[1] = new Vector2(0, 0);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);
        uv[4] = new Vector2(0, .5f);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;
        triangles[6] = 3;
        triangles[7] = 1;
        triangles[8] = 4;
        triangles[9] = 1;
        triangles[10] = 0;
        triangles[11] = 4;
        triangles[12] = 4;
        triangles[13] = 2;
        triangles[14] = 0;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GameObject meshObject = new GameObject("Mesh Object", typeof(MeshFilter), typeof(MeshRenderer));
        meshObject.transform.localScale = new Vector3(30, 30, 1);
        meshObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
