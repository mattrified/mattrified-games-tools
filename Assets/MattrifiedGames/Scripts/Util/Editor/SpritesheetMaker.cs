using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MattrifiedGames.Utility.Editor
{
    public class SpritesheetMaker : EditorWindow
    {
        public SpriteAlignment alignment;

        [MenuItem("Tools/Spritesheet Maker")]
        public static void Open()
        {
            SpritesheetMaker sm = GetWindow<SpritesheetMaker>();
            sm.Show();
        }

        public string newTextureName;

        void OnGUI()
        {
            newTextureName = EditorGUILayout.TextField("New Texture's Name", newTextureName);

            alignment = (SpriteAlignment)EditorGUILayout.EnumPopup("Alignment", alignment);

            if (GUILayout.Button("Build Spritesheet"))
            {
                List<Texture2D> textureList = new List<Texture2D>();
                foreach (Object obj in Selection.objects)
                {
                    if (obj is Texture2D)
                        textureList.Add(obj as Texture2D);
                }

                if (textureList.Count <= 0)
                    return;

                List<bool> wasReadable = new List<bool>();
                List<TextureImporterCompression> previousCompression = new List<TextureImporterCompression>();
                textureList.Sort((x, y) => string.Compare(x.name, y.name));
                foreach (Texture2D tex in textureList)
                {
                    TextureImporter ti = QuickTextureEditor.GetImporter<TextureImporter>(tex);
                    bool readable = ti.isReadable;

                    wasReadable.Add(readable);
                    previousCompression.Add(ti.textureCompression);

                    ti.isReadable = true;
                    ti.textureCompression = TextureImporterCompression.Uncompressed;
                    ti.SaveAndReimport();
                }

                Texture2D newTexture = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
                Rect[] rects = newTexture.PackTextures(textureList.ToArray(), 2, 4096 * 2);
                newTexture.name = newTextureName;

                string path = QuickTextureEditor.SaveTexture(newTexture);

                for (int i = 0; i < textureList.Count; i++)
                {
                    TextureImporter ti = QuickTextureEditor.GetImporter<TextureImporter>(textureList[i]);
                    ti.isReadable = wasReadable[i];
                    ti.textureCompression = previousCompression[i];
                    ti.SaveAndReimport();
                }

                AssetDatabase.Refresh();

                Texture2D td = (Texture2D)AssetDatabase.LoadAssetAtPath(path.Substring(path.IndexOf("Assets/")),
                    typeof(Texture2D));

                TextureImporter newTexTI = QuickTextureEditor.GetImporter<TextureImporter>(td);
                newTexTI.textureType = TextureImporterType.Sprite;
                newTexTI.spriteImportMode = SpriteImportMode.Multiple;
                List<SpriteMetaData> smdList = new List<SpriteMetaData>();
                for (int i = 0; i < rects.Length; i++)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    Rect newRect = new Rect(rects[i].x * td.width, rects[i].y * td.height,
                        rects[i].width * td.width, rects[i].height * td.height);
                    smd.rect = newRect;
                    smd.name = textureList[i].name;
                    smd.alignment = (int)alignment;
                    smdList.Add(smd);
                }
                newTexTI.spritesheet = smdList.ToArray();
                newTexTI.SaveAndReimport();
            }
        }
    }
}
