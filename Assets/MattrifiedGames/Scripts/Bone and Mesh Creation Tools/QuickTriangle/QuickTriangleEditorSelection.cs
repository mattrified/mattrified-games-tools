using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTriangleEditorSelection : QuickTriangleEditor
{
    public DrawMode currentDrawMode;
    public Color currentColor = Color.gray;

    public DrawMode copyDrawMode;
    public Color copyColor = Color.white;

    public enum DrawMode
    {
        Off = 0,
        Wireframe = 1,
        Solid = 2,
    }

    public override void OnValidate()
    {
        colliders = GetComponentsInChildren<Collider>();
    }

    private void OnDrawGizmos()
    {
        if (currentMesh != null && currentDrawMode != DrawMode.Off)
        {
            Gizmos.color = currentColor;
            if (currentDrawMode == DrawMode.Solid)
                Gizmos.DrawMesh(currentMesh, transform.position, transform.rotation, transform.localScale);
            else
                Gizmos.DrawWireMesh(currentMesh, transform.position, transform.rotation, transform.localScale);
        }

        if (lastCreatedMesh != null && copyDrawMode != DrawMode.Off)
        {
            Gizmos.color = copyColor;
            if (copyDrawMode == DrawMode.Solid)
                Gizmos.DrawMesh(lastCreatedMesh, transform.position, transform.rotation, transform.localScale);
            else
                Gizmos.DrawWireMesh(lastCreatedMesh, transform.position, transform.rotation, transform.localScale);
        }

    }
}

