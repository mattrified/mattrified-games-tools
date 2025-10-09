//-----------------------------------------------------------------------------
//  Copyright © 2016 Mattrified Games, LLC. All Rights Reserved. 
//
//  Author: Matt DeLucas
//  Date:   5/17/2016 3:32:12 PM
//-----------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace MattrifiedGames.Utility.Editor
{
    /// <summary>
    /// Editor window for quickly swapping, rearranging, and other things to textures in Unity3D.
    /// </summary>
    public class QuickTextureEditor : EditorWindow
    {
        /// <summary>
        /// A list of the current affected textures.
        /// </summary>
        List<TextureInformation> texturePositionList;

        /// <summary>
        /// If true, the new texture's size will be forced to the nearest power of two.
        /// </summary>
        bool forcePowerOfTwo = false;

        /// <summary>
        /// Width of the new texture.
        /// </summary>
        int newTexWidth = 512;

        /// <summary>
        /// Height of the new texture.
        /// </summary>
        int newTexHeight = 512;

        /// <summary>
        /// The name of the new texture to be created.
        /// </summary>
        string newTextureName = "New Texture";

        /// <summary>
        /// Operations affecting different channels.
        /// </summary>
        public enum ChannelOperations
        {
            Ignore = 0,
            Set = 1,
            Add = 2,
            Subtract = 3,
            Multiply = 4,
            Divide = 5,
        }

        public struct ChannelBlendSetup
        {
            public ChannelOperations rCU, gCU, bCU, aCU;
        }

        /// <summary>
        /// Information about each texture being used to create the new texture.
        /// </summary>
        internal class TextureInformation
        {
            /// <summary>
            /// The texture being used.
            /// </summary>
            public Texture2D texture;

            /// <summary>
            /// The x and y position of the new texture.
            /// </summary>
            public int xPos, yPos;

            /// <summary>
            /// The x and y position of the new texture.
            /// </summary>
            public int width, height;

            /// <summary>
            /// Should a multiply color be used?
            /// </summary>
            public ChannelOperations blendColorUse = ChannelOperations.Ignore;
            
            /// <summary>
            /// The color to be blended with the texture.
            /// </summary>
            public Color blendColor;

            public ChannelBlendSetup rBS = new ChannelBlendSetup() { rCU = ChannelOperations.Set },
                gBS = new ChannelBlendSetup() { gCU = ChannelOperations.Set },
                bBS = new ChannelBlendSetup() { bCU = ChannelOperations.Set },
                aBS = new ChannelBlendSetup() { aCU = ChannelOperations.Set };

            public void OnGUI(string label, ref int refWidth, ref int refHeight)
            {
                if (texture != null)
                    label = texture.name;
                texture = (Texture2D)EditorGUILayout.ObjectField(label, texture, typeof(Texture2D), false);

                if (GUILayout.Button("Set as new texture size."))
                {
                    refWidth = width;
                    refHeight = height;
                }

                if (texture == null)
                {
                    Vector2 s = new Vector2(width, height);
                    s = EditorGUILayout.Vector2Field("Size", s);
                    width = Mathf.Max(1, Mathf.RoundToInt(s.x));
                    height = Mathf.Max(1, Mathf.RoundToInt(s.y));
                }
                else
                {
                    width = texture.width;
                    height = texture.height;
                }

                blendColorUse = (ChannelOperations)EditorGUILayout.EnumPopup("Blend Color Usage", blendColorUse);
                if (blendColorUse != ChannelOperations.Ignore)
                    blendColor = EditorGUILayout.ColorField(blendColor);
                else
                    blendColor = Color.white;

                Vector2 v = new Vector2(xPos, yPos);
                v = EditorGUILayout.Vector2Field("Pos", v);
                xPos = Mathf.RoundToInt(v.x);
                yPos = Mathf.RoundToInt(v.y);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                GUILayout.Label("");
                GUI.color = Color.red;
                GUILayout.Label("R");

                GUI.color = Color.green;
                GUILayout.Label("G");

                GUI.color = Color.blue;
                GUILayout.Label("B");

                GUI.color = Color.white;
                GUILayout.Label("A");
                EditorGUILayout.EndVertical();

                ChangeBlendSetup("R channel", ref rBS, Color.red);
                ChangeBlendSetup("G channel", ref gBS, Color.green);
                ChangeBlendSetup("B channel", ref bBS, Color.blue);
                ChangeBlendSetup("A channel", ref aBS, Color.white);

                EditorGUILayout.EndHorizontal();
            }

            private void ChangeBlendSetup(string p, ref ChannelBlendSetup bS, Color guiColor)
            {
                EditorGUILayout.BeginVertical();
                GUI.color = guiColor;
                GUILayout.Label(p);
                GUI.color = Color.white;
                
                bS.rCU = (ChannelOperations)EditorGUILayout.EnumPopup(bS.rCU);
                bS.gCU = (ChannelOperations)EditorGUILayout.EnumPopup(bS.gCU);
                bS.bCU = (ChannelOperations)EditorGUILayout.EnumPopup(bS.bCU);
                bS.aCU = (ChannelOperations)EditorGUILayout.EnumPopup(bS.aCU);
                
                EditorGUILayout.EndVertical();
            }

            internal void EditColor(ref Color colorOutput, ref Color colorInput)
            {
                EditChannel(ref colorOutput.r, ref colorInput, rBS);
                EditChannel(ref colorOutput.g, ref colorInput, gBS);
                EditChannel(ref colorOutput.b, ref colorInput, bBS);
                EditChannel(ref colorOutput.a, ref colorInput, aBS);
            }

            private void EditChannel(ref float outputValue, ref Color inputColor, ChannelBlendSetup bs)
            {
                EditChannel(ref outputValue, ref inputColor.r, bs.rCU);
                EditChannel(ref outputValue, ref inputColor.g, bs.gCU);
                EditChannel(ref outputValue, ref inputColor.b, bs.bCU);
                EditChannel(ref outputValue, ref inputColor.a, bs.aCU);
            }

            private void EditChannel(ref float output, ref float input, ChannelOperations channelUsage)
            {
                switch (channelUsage)
                {
                    case ChannelOperations.Set:
                        output = input;
                        break;
                    case ChannelOperations.Add:
                        output += input;
                        break;
                    case ChannelOperations.Divide:
                        output /= input;
                        break;
                    case ChannelOperations.Multiply:
                        output *= input;
                        break;
                    case ChannelOperations.Subtract:
                        output -= input;
                        break;
                    case ChannelOperations.Ignore:
                        return;
                }
            }
        }

        

        // Add menu named "My Window" to the Window menu
        [MenuItem("Tools/Quick Texture Editor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            QuickTextureEditor window = (QuickTextureEditor)EditorWindow.GetWindow(typeof(QuickTextureEditor));
            window.Show();
        }

        /// <summary>
        /// On GUI function that displays information in the editor.
        /// </summary>
        void OnGUI()
        {
            OnGUICombineTextures();
        }

        /// <summary>
        /// Quickly gets the importer of a specified asset
        /// </summary>
        /// <typeparam name="T">The type of importer to be used.</typeparam>
        /// <param name="asset">The asset whose importer is being referenced.</param>
        /// <returns>The importer, converted to the requested type.</returns>
        public static T GetImporter<T>(UnityEngine.Object asset) where T : AssetImporter
        {
            return (T)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(asset));
        }

        private void SetupList<T>(ref List<T> list, int p)
        {
            if (list == null)
                list = new List<T>();
            while (list.Count <= p)
                list.Add(default(T));
        }

        private T GetFromList<T>(ref List<T> list, int p)
        {
            SetupList(ref list, p);
            return list[p];
        }

        private void DefineTexturePose(int index)
        {
            SetupList(ref texturePositionList, index);
            if (texturePositionList[index] == null)
                texturePositionList[index] = new TextureInformation();

            texturePositionList[index].OnGUI("Texture " + index, ref newTexWidth, ref newTexHeight);
        }

        private static Color DivideColor(Color c)
        {
            return new Color(1f / c.r, 1f / c.g, 1f / c.b, 1f / c.a);
        }

        Vector2 scroll;
        private void OnGUICombineTextures()
        {
            // Defines information about the new texture.
            newTextureName = EditorGUILayout.TextField("New Texture Name", newTextureName);

            forcePowerOfTwo = EditorGUILayout.Toggle("Force Power of 2", forcePowerOfTwo);
            if (forcePowerOfTwo)
            {
                newTexWidth = Mathf.ClosestPowerOfTwo(EditorGUILayout.IntField("Width", newTexWidth));
                newTexHeight = Mathf.ClosestPowerOfTwo(EditorGUILayout.IntField("Height", newTexHeight));
            }
            else
            {
                newTexWidth = EditorGUILayout.IntField("Width", newTexWidth);
                newTexHeight = EditorGUILayout.IntField("Height", newTexHeight);
            }

            EditorGUILayout.Separator();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            if (texturePositionList == null)
                texturePositionList = new List<TextureInformation>();
            for (int i = 0; i < texturePositionList.Count; i++)
            {
                DefineTexturePose(i);
            }


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Texture"))
            {
                texturePositionList.Add(new TextureInformation());
                return;
            }
            if (GUILayout.Button("Remove Texture"))
            {
                texturePositionList.RemoveAt(texturePositionList.Count - 1);
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Separator();

            if (GUILayout.Button("Save Texture"))
            {
                int textureCount = texturePositionList.Count;

                Texture2D newTex = new Texture2D(newTexWidth, newTexHeight);
                newTex.name = string.IsNullOrEmpty(newTextureName) ? "New Texture" : newTextureName; 
                Color[] mainColors = new Color[newTex.width * newTex.height];
                newTex.SetPixels(mainColors);

                List<TextureInformation> pulledTextures = new List<TextureInformation>();
                for (int i = 0; i < textureCount; i++)
                {
                    TextureInformation pos = GetFromList(ref texturePositionList, i);
                    if (pos == null)
                        continue;
                    else if (pos.texture == null)
                    {
                        pos.texture = new Texture2D(pos.width, pos.height);
                        pos.texture.name = "Texture " + i;
                        Color[] c = new Color[pos.width * pos.height];
                        for (int j = 0; j < c.Length; j++)
                            c[j] = pos.blendColor;
                        pos.texture.SetPixels(c);
                        pos.texture.Apply();
                    }

                    if (pos.texture.width + pos.xPos > newTex.width ||
                        pos.texture.height + pos.yPos > newTex.height)
                    {
                        Debug.LogWarning(pos.texture.name + " will not fit into new texture.  Skipping.");
                        continue;
                    }

                    pulledTextures.Add(pos);
                }

                for (int i = 0; i < pulledTextures.Count; i++)
                {
                    EditorUtility.DisplayProgressBar("Saving Texture", "Working on Texture " + i, (i + 1) / (pulledTextures.Count));

                    TextureImporter ti = GetImporter<TextureImporter>(pulledTextures[i].texture);
                    bool wasReadable = ti.isReadable;
                    TextureImporterType importType = ti.textureType;
                    bool wasNormal = importType == TextureImporterType.NormalMap;

                    if (wasReadable != true)
                    {
                        ti.isReadable = true;
                        ti.SaveAndReimport();
                    }

                    if (wasNormal)
                    {
                        ti.textureType = TextureImporterType.Default;
                        ti.SaveAndReimport();
                    }


                    Color[] pulledColors = pulledTextures[i].texture.GetPixels();

                    if (pulledTextures[i].blendColorUse != ChannelOperations.Ignore)
                    {
                        for (int c = 0; c < pulledColors.Length; c++)
                        {
                            switch (pulledTextures[i].blendColorUse)
                            {
                                case ChannelOperations.Set:
                                    pulledColors[c] = pulledTextures[i].blendColor;
                                    break;
                                case ChannelOperations.Add:
                                    pulledColors[c] += pulledTextures[i].blendColor;
                                    break;
                                case ChannelOperations.Divide:
                                    pulledColors[c] *= DivideColor(pulledTextures[i].blendColor);
                                    break;
                                case ChannelOperations.Multiply:
                                    pulledColors[c] *= pulledTextures[i].blendColor;
                                    break;
                            }
                        }
                    }

                    Color[] colorsToModify =
                        newTex.GetPixels(pulledTextures[i].xPos, pulledTextures[i].yPos, pulledTextures[i].texture.width, pulledTextures[i].texture.height);
                    
                    // Adds these colors instead of setting.  Slower, but allows for combining channels or for combining reasons.
                    for (int c = 0; c < colorsToModify.Length; c++)
                        pulledTextures[i].EditColor(ref colorsToModify[c], ref pulledColors[c]);

                    newTex.SetPixels(pulledTextures[i].xPos, pulledTextures[i].yPos, pulledTextures[i].texture.width, pulledTextures[i].texture.height,
                        colorsToModify);

                    if (ti.isReadable != wasReadable)
                    {
                        ti.isReadable = wasReadable;
                        ti.SaveAndReimport();
                    }

                    if (wasNormal)
                    {
                        ti.textureType = TextureImporterType.NormalMap;
                        ti.SaveAndReimport();
                    }
                }

                SaveTexture(newTex);

                EditorUtility.ClearProgressBar();
            }
        }

        public static string SaveTexture(Texture2D texture2D, string path = "")
        {
            byte[] bytes = texture2D.EncodeToPNG();

            if (string.IsNullOrEmpty(path))
                path = Application.dataPath + "/" + texture2D.name + ".png";

            File.WriteAllBytes(path, bytes);

            AssetDatabase.Refresh();

            return path;
        }
    }
}
