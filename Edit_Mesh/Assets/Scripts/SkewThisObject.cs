using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  Attach this object to individual objects you want to be skewed.
    There must be a separate script (see "SkewManager") to call upon functions to skew.

    Note that bad behavior may occur if rotated and scale not (1,1,1).
    Further errors may occur if a child of another parent that is rotated (which may cause inaccurate lossyScale values).
*/

public class SkewThisObject : MonoBehaviour {

    Vector3 lossyScale = Vector3.zero;
    Vector3[] newVerts;
    Vector3[] originalVerts;
    int vertLength = 0;
    MeshFilter mf;
    Vector3 originalPosition;
    Vector3 originalBase;
    Vector3 additiveBase;
    float offsetPos = 1.0f;

    void Start()
    {
        mf = this.GetComponent<MeshFilter>();
        if (mf == null)
            return;

        vertLength = mf.mesh.vertices.Length;
        newVerts = new Vector3[mf.mesh.vertices.Length];
        originalVerts = new Vector3[mf.mesh.vertices.Length];
        System.Array.Copy(mf.mesh.vertices, newVerts, mf.mesh.vertices.Length);
        System.Array.Copy(mf.mesh.vertices, originalVerts, mf.mesh.vertices.Length);

        originalPosition = this.transform.position;

        lossyScale = this.transform.lossyScale;
    }

    public void InitializeBase(Vector3 bottomPos)
    {
        originalBase = bottomPos;
        additiveBase = Vector3.zero;
    }

    public void MoveBase(Vector3 bottomPos)
    {
        additiveBase = new Vector3(
            -originalBase.x + bottomPos.x,
            originalBase.y,
            -originalBase.z + bottomPos.z);
    }

    public void Move(Vector3 topPos, Vector3 bottomPos)
    {
        offsetPos = this.transform.position.y / (topPos.y - bottomPos.y);
        this.transform.position = new Vector3(
            originalPosition.x + additiveBase.x + (topPos.x - bottomPos.x) * offsetPos,
            originalPosition.y,
            originalPosition.z + additiveBase.z + (topPos.z - bottomPos.z) * offsetPos);
    }

    public void SkewMe(float angleX, float angleY, float angleZ)
    {
        // shear = tan(angle) in degrees, not radians. For example, tan(45) = 1.0
        float xMul = lossyScale.y / lossyScale.x;
        float yMul = lossyScale.y / lossyScale.y;
        float zMul = lossyScale.y / lossyScale.z;

        Vector3 shear = new Vector3
            (angleX * xMul,
            angleY * yMul,
            angleZ * zMul);

        if (originalVerts == null || newVerts == null)
            return;
        System.Array.Copy(originalVerts, newVerts, originalVerts.Length);

        for (int i = 0; i < vertLength; i++)
        {
            Quaternion newRotation = new Quaternion();// g.transform.rotation;
            newRotation.eulerAngles = this.transform.rotation.eulerAngles;
            newVerts[i] = newRotation * (newVerts[i]);
            newVerts[i] = newVerts[i] + shear * newVerts[i].y;
            newRotation.eulerAngles = -this.transform.rotation.eulerAngles;
            newVerts[i] = (newRotation) * (newVerts[i]);
        }

        mf.mesh.vertices = newVerts;
    }
}
