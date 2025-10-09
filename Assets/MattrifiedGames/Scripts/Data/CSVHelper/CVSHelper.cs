#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace MattrifiedGames.Utility
{
    [CreateAssetMenu()]
    public class CVSHelper : ScriptableObject
    {
        public TextAsset definitionCSV;
        public TextAsset mainCSV;

        const string BRACKET_LEFT = "\n{\n";
        const string BRACK_RIGHT = "\n}\n";

        [SerializeField()]
        string nameSpace = string.Empty;

        [SerializeField()]
        string directory = "Assets/";

        [SerializeField()]
        string className;

        [SerializeField()]
        List<string> usingClasses = new List<string>();

        [SerializeField()]
        List<string> enums = new List<string>();

        [SerializeField()]
        List<string> enumTypes = new List<string>();

        [SerializeField()]
        List<string> fields = new List<string>();

        [SerializeField()]
        List<string> previousLines = new List<string>();

        [ContextMenu("Create Definition Test")]
        void CreateTestDef()
        {
            CreateDefinition();
        }

        [ContextMenu("Create Definition CS")]
        void CreateDef()
        {
            CreateDefinition(".cs");
        }

        void CreateDefinition(string fileType = ".txt")
        {
            if (definitionCSV == null)
                return;

            try
            {
                ParseDefinition(definitionCSV.text, mainCSV.text);

                int tab = 0;
                string file = BeingWithUsingStatements();
                file = WriteNamespace(file, ref tab);
                file = WriteClassName(file, ref tab);
                file = WriteEnums(file, ref tab);

                file = WriteFields(file, ref tab);

                file = string.Format(file, BRACKET_LEFT, BRACK_RIGHT);

                UnityFileWriter.WriteFile(directory, className + fileType, file);
            } 
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        private string WriteFields(string file, ref int tab)
        {
            string f = string.Empty;
            for (int i = 0; i < fields.Count; i++)
            {
                f += Tab(tab) + fields[i] + "\n";
            }
            return string.Format(file, "{0}", "{1}", f);
        }

        string BeingWithUsingStatements()
        {
            string s = string.Empty;
            for (int i = 0; i < usingClasses.Count; i++)
            {
                s += "using " + usingClasses[i] + ";\n";
            }

            s += "\n";

            return s;
        }

        string WriteNamespace(string usingString, ref int tabValue)
        {
            if (string.IsNullOrEmpty(nameSpace))
                return usingString + "{2}";

            usingString += "namespace " + nameSpace + "{0}{2}{1}";

            tabValue++;

            return usingString;
        }

        string WriteClassName(string currentString, ref int tabValue)
        {
            string classString = Tab(tabValue) + "public class " + className + " : ScriptableObject{0}{2}{1}";

            currentString = string.Format(currentString, Tab(tabValue) + "{0}", Tab(tabValue) + "{1}", classString);
            tabValue++;

            return currentString;
        }

        string WriteEnums(string currentString, ref int tabValue)
        {
            string e = string.Empty;
            for (int i = 0; i < enums.Count; i++)
                e += CreateEnum(i, ref tabValue);
            e += "\n{2}";

            currentString = string.Format(currentString, Tab(tabValue) + "{0}", Tab(tabValue) + "{1}", e);

            return currentString;
        }

        string CreateEnum(int index, ref int tabValue)
        {
            string enumLine = enums[index];
            string[] enumItems = enumLine.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            string enumItem = Tab(tabValue) + "public enum " + enumItems[0] + Tab(tabValue) + "{0}";
            for (int i = 1; i < enumItems.Length; i++)
            {
                enumItem += "\n" + Tab(tabValue + 1) + enumItems[i] + " = " + (i - 1).ToString() + ",";
            }
            enumItem += Tab(tabValue) + "{1}";

            return enumItem;
        }

        public string Tab(int tabValue)
        {
            string s = string.Empty;
            for (int i = 0; i < tabValue; i++)
                s += '\t';
            return s;
        }

        private void ParseDefinition(string definitionText, string assetText)
        {
            List<string[]> splitLines = new List<string[]>();
            List<string> lines = new List<string>(definitionText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            foreach (string line in lines)
            {
                splitLines.Add(line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            // Finds the class name
            string[] classLine = splitLines.Find(x => x[0] == "class");
            if (classLine != null)
                className = classLine[1];

            // Finds using information
            string[] directoryLine = splitLines.Find(x => x[0] == "folder");
            if (directoryLine != null)
                directory = directoryLine[1];

            // Finds using information
            string[] usingLine = splitLines.Find(x => x[0] == "using");
            if (usingClasses != null)
            {
                usingClasses = new List<string>(usingLine);
                usingClasses.RemoveAt(0);
                usingClasses.RemoveAll(x => string.IsNullOrEmpty(x));
            }

            enums = new List<string>(lines.FindAll(x => x.StartsWith("enum")));
            enumTypes = new List<string>();
            for (int i = 0; i < enums.Count; i++)
            {
                enums[i] = enums[i].Replace("enum,", "");
                string prefix = enums[i].Substring(0, enums[i].IndexOf(','));
                if (!enumTypes.Contains(prefix))
                    enumTypes.Add(prefix);
            }

            for (int i = 0; i < enumTypes.Count; i++)
            {
                int firstIndex = -1;
                for (int j = 0; j < enums.Count; j++)
                {
                    if (enums[j].StartsWith(enumTypes[i]))
                    {
                        if (firstIndex < 0)
                            firstIndex = j;
                        else
                        {
                            enums[firstIndex] += enums[j].Replace(enumTypes[i], "");
                            enums.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }

            // Parses the class information
            List<string> assetLines = new List<string>(assetText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
            string[] splitFirstLine = assetLines[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            fields = new List<string>();
            for (int i = 1; i < splitFirstLine.Length; i++)
            {
                string f = "public {1} {0};";
                string[] typeSplit = splitFirstLine[i].Split(':');

                typeSplit[0] = typeSplit[0].Replace(" ", "");

                fields.Add(string.Format(f, typeSplit[0], typeSplit[1]));
            }
        }

        [ContextMenu("Create Assets")]
        void CreateAssets()
        {
            CreateAssets(mainCSV.text);
        }

        void CreateAssets(string txt)
        {
            string[] lines = txt.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            string[] fieldTypes = new string[fields.Count];
            string[] fieldNames = new string[fields.Count];

            for (int i = 0; i < fields.Count; i++)
            {
                string[] s = fields[i].Split(' ');
                fieldTypes[i] = s[1];
                fieldNames[i] = s[2].Replace(";", "");
            }

            if (previousLines.Count == 0)
                previousLines.Add(lines[0]);

            for (int i = 1; i < lines.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Creating Assets", "Line " + (i + 1), Mathf.InverseLerp(0, lines.Length - 1, i));

                if (i >= previousLines.Count)
                {
                    previousLines.Add(lines[i]);
                }
                else
                {
                    if (previousLines[i] == lines[i])
                        continue;

                    previousLines[i] = lines[i];
                }

                string[] items = lines[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (items.Length == 0 || string.IsNullOrEmpty(items[0]) || items[0].StartsWith("//") || items[0].StartsWith("--"))
                    continue;

                string soName = items[0];

                bool createNew = false;
                ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/" + directory + "/" + soName + ".asset");
                if (so == null)
                {
                    so = ScriptableObject.CreateInstance(className);
                    so.name = soName;
                    createNew = true;
                }

                var type = so.GetType();
                for (int j = 1; j < items.Length; j++)
                {
                    Debug.Log(fieldNames[j - 1]);
                    object obj;
                    if (ConvertType(fieldTypes[j - 1], items[j], out obj))
                    {
                        type.GetField(fieldNames[j - 1]).SetValue(so, obj);
                    }
                }

                if (createNew)
                    AssetDatabase.CreateAsset(so, "Assets/" + directory + "/" + so.name + ".asset");

                UnityEditor.EditorUtility.SetDirty(so);
            }

            EditorUtility.ClearProgressBar();
        }

        private bool ConvertType(string fType1, string item, out object result)
        {
            Debug.Log(fType1 + ", " + item);
            switch (fType1)
            {
                case "bool":
                case "Bool":
                case "Boolean":
                    result = bool.Parse(item.ToLower());
                    return true;
                case "byte":
                case "Byte":
                    result = byte.Parse(item);
                    return true;
                case "short":
                    result = short.Parse(item);
                    return true;
                case "int":
                case "Int32":
                    result = int.Parse(item);
                    return true;
                case "FP":
                    result = TrueSync.FP.FromFloat(float.Parse(item));
                    return true;
                case "float":
                    result = float.Parse(item);
                    return true;
                case "GameObject":
                case "Object":
                    result = null;
                    return false;
                default:
                    Type t = Type.GetType(fType1);
                    if (t == null)
                    {
                        if (!string.IsNullOrEmpty(nameSpace))
                        {
                            t = Type.GetType(nameSpace + "+" + className + "+" + fType1, false, true);
                        }
                        else
                        {
                            t = Type.GetType(className + "+" + fType1, false, true);
                        }
                    }

                    if (t == null)
                    {
                        Debug.Log("Type not found:  " + fType1);
                        result = null;
                        return false;
                    }

                    if (t.IsEnum)
                    {
                        result = Enum.Parse(t, item);
                        Debug.Log("ENUM RESULT:  " + result);
                        return true;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }
            }
        }
    }
}
#endif