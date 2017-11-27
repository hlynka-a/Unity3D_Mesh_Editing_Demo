using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  Attach this script to an object with a mesh,
    and it will generate game objects at each vertex upon running the game.
    Move the objects generated at each vertex, and this script will update the mesh according to your positions.

    Note that this does not include logic to move each vertex object in the game window,
    they must be moved in the editor window while running the game.

    Note that this script does not handle saving the modified mesh after ending the game.

    For vertex objects to appear in correct positions, the original mesh transform must have (1,1,1) scaling, and (0,0,0) rotation and position.
    Else, the vertex objects will appear offset, but still have the same function when moved.
 */


public class MoveVertices : MonoBehaviour {

    GameObject[] followMeArray;

    Vector3[] originalVerts;
    int vertLength;
    MeshFilter mf;
    Vector3[] newVerts;

    void Start()
    {
        InitializeVertexPoints();
    }

    void Update()
    {
        UpdateVertices();
    }

    /* Set GameObjects at the vertices of the subject mesh. Allows user to drag vertices around at runtime (not saved outside game or in editor after playing game).*/
    void InitializeVertexPoints()
    {
        mf = this.GetComponent<MeshFilter>();
        vertLength = mf.mesh.vertices.Length;
        Debug.Log("this many vertices = " + vertLength);

        newVerts = new Vector3[mf.mesh.vertices.Length];
        followMeArray = new GameObject[vertLength];

        for (int i = 0; i < vertLength; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = "vertex_" + i;
            g.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            g.transform.position = mf.mesh.vertices[i];
            followMeArray[i] = g;

            /*  Each quad in a mesh has its own vertex points, even if the points are shared with connected quads. 
                This means many duplicate vertices exist in the data.
                For this script, it is easier to remove duplicates at initialization.
            */
            for (int j = 0; j < i; j++)
            {
                if (followMeArray[j].transform.position == followMeArray[i].transform.position)
                {
                    GameObject.Destroy(g);
                    followMeArray[i] = followMeArray[j];
                    continue;
                }
            }
        }

        System.Array.Copy(mf.mesh.vertices, newVerts, mf.mesh.vertices.Length);
        originalVerts = new Vector3[mf.mesh.vertices.Length];
        System.Array.Copy(mf.mesh.vertices, originalVerts, mf.mesh.vertices.Length);
    }

    void UpdateVertices()
    {
        // moving each vertex position one by one, based on a parent object (which parent objects move needs to be set elsewhere)
        System.Array.Copy(originalVerts, newVerts, originalVerts.Length);
        for (int i = 0; i < vertLength; i++)
        {
            newVerts[i] = followMeArray[i].transform.position;
        }
        mf.mesh.vertices = newVerts;

        /* If a box of one of the vertices moves,
         *      - move all boxes that correspond to it.
         *
         * Check boxes and respective verticies
         *      - move each vertex to same position as respective box.
         *      
         * Mesh is (1,1,1), even if higher layer (transform) is scaled differently...
         *      - consider to scale/position mesh at start, and keep transform at (1,1,1), might simplify code logic elsewhere
         */
    }
}
