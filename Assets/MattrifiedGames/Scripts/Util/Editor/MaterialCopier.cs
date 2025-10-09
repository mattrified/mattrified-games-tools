using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MaterialCopier : EditorWindow {

    public Material matA, matB;

    [MenuItem("Tools/Material Copier")]
    static void Open()
    {
        EditorWindow.GetWindow<MaterialCopier>();
    }

    private void OnGUI()
    {
        matA = (Material)EditorGUILayout.ObjectField("Material A", matA, typeof(Material), false);
        matB = (Material)EditorGUILayout.ObjectField("Material B", matB, typeof(Material), false);

        if (matA == null || matB == null)
            return;

        if (matA.shader != matB.shader)
        {
            if (GUILayout.Button("Copy " + matA.name + " shader to " + matB.name))
                matB.shader = matA.shader;
            if (GUILayout.Button("Copy " + matB.name + " shader to " + matA.name))
                matA.shader = matB.shader;
        }

        if (GUILayout.Button(string.Format("Copy {0} properties to {1}", matA.name, matB.name)))
        {
            // Gets all of the texture properties
            string[] matBName = matB.GetTexturePropertyNames();

            Texture[] matBOldTextures = new Texture[matBName.Length];
            for (int i = 0; i < matBName.Length; i++)
            {
                matBOldTextures[i] = matB.GetTexture(matBName[i]);
            }

            matB.CopyPropertiesFromMaterial(matA);
            for (int i = 0; i < matBName.Length; i++)
            {
                matB.SetTexture(matBName[i], matBOldTextures[i]);
            }
        }

        if (GUILayout.Button(string.Format("Copy {1} properties to {0}", matA.name, matB.name)))
        {
            // Gets all of the texture properties
            string[] matAName = matA.GetTexturePropertyNames();

            Texture[] matAOldTextures = new Texture[matAName.Length];
            for (int i = 0; i < matAName.Length; i++)
            {
                matAOldTextures[i] = matA.GetTexture(matAName[i]);
            }

            matA.CopyPropertiesFromMaterial(matB);
            for (int i = 0; i < matAName.Length; i++)
            {
                matA.SetTexture(matAName[i], matAOldTextures[i]);
            }
        }
    }
}