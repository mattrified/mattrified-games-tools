using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ParticleSheetCreator
{
    [MenuItem("Assets/Create/Particle Spritesheet")]
    public static void CreateParticleSheet()
    {
        List<Texture2D> textureList = new List<Texture2D>();

        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(path);

        var textures = AssetDatabase.FindAssets("t:Texture2D", new string[] { path });

        foreach (var guid in textures)
        {
            var texPath = AssetDatabase.GUIDToAssetPath(guid);
            Debug.Log(texPath);

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            textureList.Add(texture);
        }

        int maxSize = 0;
        for (int i = 0; i < textureList.Count; i++)
        {
            maxSize = Mathf.Max(maxSize, textureList[i].height, textureList[i].width);
        }

        // Calculate sheet size.
        int root = Mathf.CeilToInt(Mathf.Sqrt(textureList.Count));

        Texture2D p = new Texture2D(maxSize * root, maxSize * root);
        p.SetPixels32(new Color32[p.width * p.height]);

        int index = 0;
        for (int i = 0; i < root; i++)
        {
            for (int j = 0; j < root; j++)
            {
                if (index >= textureList.Count)
                    break;

                int mL = (maxSize - textureList[i].width) / 2;
                int mB = (maxSize - textureList[i].height) / 2;

                p.SetPixels(j * maxSize + mL, (root - 1 - i) * maxSize + mB, textureList[index].width, textureList[index].height, textureList[index].GetPixels());

                index++;
            }
        }

        byte[] bytes = p.EncodeToPNG();
        System.IO.File.WriteAllBytes(path + "/" + textureList[0].name + " sheet.png", bytes);
        AssetDatabase.ImportAsset(path + "/" + textureList[0].name + " sheet.png");
    }
}