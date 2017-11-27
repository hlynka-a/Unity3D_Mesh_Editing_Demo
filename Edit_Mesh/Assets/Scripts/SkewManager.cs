using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkewManager : MonoBehaviour {

    public SkewThisObject[] objectsToSkew;
    public GameObject topObject = null;
    Vector3 topPosition = Vector3.zero;
    Vector3 originalTopPosition = Vector3.zero;
    public GameObject bottomObject = null;
    Vector3 bottomPosition = Vector3.zero;
    Vector3 originalBottomPosition = Vector3.zero;
    float newAnX = 0f;
    float newAnZ = 0f;
    float angleX = 0f;
    float angleZ = 0f;

    void Start()
    {
        topPosition = topObject.transform.position;
        bottomPosition = bottomObject.transform.position;
        originalTopPosition = topPosition;
        originalBottomPosition = bottomPosition;

        if (objectsToSkew.Length == 0) { 
            GetObjectsToSkew();
        }

        foreach (SkewThisObject sk in objectsToSkew)
        {
            sk.InitializeBase(bottomObject.transform.position);
        }
    }

    void GetObjectsToSkew()
    {
        objectsToSkew = this.gameObject.GetComponentsInChildren<SkewThisObject>();
    }

    void Update()
    {
        if (topPosition != topObject.transform.position || bottomPosition != bottomObject.transform.position)
        {
            topPosition = topObject.transform.position;
            bottomPosition = bottomObject.transform.position;
            if (topPosition.x - bottomPosition.x != 0)
                newAnX = Mathf.Atan((topPosition.y - bottomPosition.y) / (topPosition.x - bottomPosition.x));
            else
                newAnX = 1.5708f;
            if (topPosition.z - bottomPosition.z != 0)
                newAnZ = Mathf.Atan((topPosition.y - bottomPosition.y) / (topPosition.z - bottomPosition.z));
            else
                newAnZ = 1.5708f;
            angleX = Mathf.Tan(-newAnX + 1.5708f); //90 deg = 1.5708f rad
            angleZ = Mathf.Tan(-newAnZ + 1.5708f);

            /*
             Skewing a 3D object... 
             Each game object has a 'transform' matrix (think OpenGL), that can be modified even without OpenGL.

             https://docs.unity3d.com/ScriptReference/Matrix4x4.html
             http://answers.unity3d.com/questions/184833/skew-an-imported-mesh.html
             */

            foreach (SkewThisObject sk in objectsToSkew)
            {
                sk.SkewMe(angleX, 0f, angleZ);
                sk.MoveBase(bottomPosition);
                sk.Move(topPosition, bottomPosition);
            }

        }
    }
}
