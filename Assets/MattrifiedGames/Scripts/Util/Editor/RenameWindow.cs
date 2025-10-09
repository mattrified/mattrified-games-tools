using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RenameToolWindow : EditorWindow
{
    /// <summary>
    /// Renames the object.
    /// </summary>
    List<string> selectionArray;

    /// <summary>
    /// The base name of the object.
    /// </summary>
    public RenameToggle baseName;

    /// <summary>
    /// The base name of the object.
    /// </summary>
    public RenameToggle removeText;


    /// <summary>
    /// The prefix before the object.
    /// </summary>
    public RenameToggle prefix;

    /// <summary>
    /// The suffix added to the object
    /// </summary>
    public RenameToggle suffix;

    // Should find/replace be used.
    public bool useReplacement;
    public string stringToReplace;
    public string stringToReplaceWith;

    public bool increment = true;

    public int incrementStart = 0;

    /// <summary>
    /// The padding for the incrementation.
    /// </summary>
    public int incrementPadding;

    /// <summary>
    /// How much does the item increment
    /// </summary>
    public int incrementValueCount;

    /// <summary>
    /// Initializes and opens a new instance of the window.
    /// </summary>
    [MenuItem("Window/Rename Asset Window")]
    public static void Init()
    {
        RenameToolWindow window = (RenameToolWindow)EditorWindow.GetWindow(typeof(RenameToolWindow));
        window.name = typeof(RenameToolWindow).Name;
    }

    void OnGUI()
    {
        SetRename(ref baseName, "Base Name");
        SetRename(ref prefix, "Prefix");
        SetRename(ref suffix, "Suffix");
        SetRename(ref removeText, "Remove");

        useReplacement = EditorGUILayout.Foldout(useReplacement, "Use Replacement");
        if (useReplacement)
        {
            stringToReplace = EditorGUILayout.TextField("String to Replace", stringToReplace);
            stringToReplaceWith = EditorGUILayout.TextField("String to Replace With", stringToReplaceWith);
        }

        increment = EditorGUILayout.Foldout(increment, "Increment Parameters");
        if (increment)
        {
            incrementStart = EditorGUILayout.IntField("Increment Start", incrementStart);
            incrementPadding = EditorGUILayout.IntField("Increment Padding", incrementPadding);
            incrementValueCount = Mathf.Max(1, EditorGUILayout.IntField("Increment Value", incrementValueCount));
        }

        if (GUILayout.Button("Rename Asset"))
            RenameAsset();

        if (GUILayout.Button("Rename Sub-Asset"))
            RenameSubAsset();

        if (GUILayout.Button("Rename Selected Objects"))
            RenameObjects(false);

        if (GUILayout.Button("Rename Selected Objects and Children"))
            RenameObjects(true);
    }

    private void RenameObjects(bool renameChildren)
    {
        var objects = Selection.gameObjects;
        if (objects == null || objects.Length == 0)
        {
            Debug.Log("No items selected.");
            return;
        }

        List<GameObject> renamedObjects = new List<GameObject>();
        int itemCount = 0;
        for (int i = 0; i < objects.Length; i++)
        {
            RenameObject(objects[i], renamedObjects, renameChildren, ref itemCount);
        }
    }

    private void RenameObject(GameObject gameObject, List<GameObject> renamedObjects, bool renameChildren, ref int count)
    {
        if (renamedObjects.Contains(gameObject))
            return;


        string name = gameObject.name;
        // Removes from the file name first.
        if (removeText.use)
            name = name.Replace(removeText.text, "");

        // Replaces text from the file name next.
        if (useReplacement)
            name = name.Replace(stringToReplace, stringToReplaceWith);

        if (baseName.use)
            name = baseName.text;
        if (prefix.use)
            name = prefix.text + name;
        if (suffix.use)
            name += suffix.text;

        if (increment)
        {
            name += (incrementStart + count * incrementValueCount).ToString().PadLeft(Mathf.Max(incrementPadding, selectionArray.Count.ToString().Length), '0');
            count++;
        }

        gameObject.name = name;
        renamedObjects.Add(gameObject);
        UnityEditor.EditorUtility.SetDirty(gameObject);

        if (renameChildren)
        {
            foreach (Transform t in gameObject.transform)
            {
                RenameObject(t.gameObject, renamedObjects, renameChildren, ref count);
                
            }
        }
    }

    private void SetRename(ref RenameToggle toggle, string label)
    {
        toggle.use = EditorGUILayout.Foldout(toggle.use, label);
        if (toggle.use)
            toggle.text = EditorGUILayout.TextField(toggle.text);
    }

    private void RenameSubAsset()
    {
        var objects = Selection.objects;
        if (objects == null || objects.Length == 0)
        {
            Debug.LogWarning("No items selected.");
            return;
        }

        List<Object> subObjectList = new List<Object>();
        for (int i = 0; i < objects.Length; i++)
        {
            if (AssetDatabase.IsSubAsset(objects[i]))
                subObjectList.Add(objects[i]);
        }
        subObjectList.Sort((x, y) => string.Compare(x.name, y.name));

        for (int i = 0; i < subObjectList.Count; i++)
        {
            string newFileName = subObjectList[i].name;

            // Removes from the file name first.
            if (removeText.use)
                newFileName = newFileName.Replace(removeText.text, "");

            // Replaces text from the file name next.
            if (useReplacement)
                newFileName = newFileName.Replace(stringToReplace, stringToReplaceWith);

            if (baseName.use)
                newFileName = baseName.text;
            if (prefix.use)
                newFileName = prefix.text + newFileName;
            if (suffix.use)
                newFileName += suffix.text;

            if (increment)
                newFileName += (incrementStart + i * incrementValueCount).ToString().PadLeft(Mathf.Max(incrementPadding, selectionArray.Count.ToString().Length), '0');

            Debug.Log("Renaming:  " + subObjectList[i].name + " to " + newFileName);

            if (string.IsNullOrEmpty(newFileName))
                continue;

            subObjectList[i].name = newFileName;

            AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(subObjectList[i]));
            EditorUtility.SetDirty(subObjectList[i]);
        }
    }

    private void RenameAsset()
    {
        var objects = Selection.objects;
        if (objects == null || objects.Length == 0)
        {
            Debug.LogWarning("No items selected.");
            return;
        }

        selectionArray = new List<string>();
        for (int i = 0; i < objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objects[i]);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("No asset path found for " + objects[i].name);
            }
            else
            {
                selectionArray.Add(path);
            }
        }
        selectionArray.Sort();

        for (int i = 0; i < selectionArray.Count; i++)
        {
            int periodIndex = selectionArray[i].LastIndexOf('.');
            int lastSlash = selectionArray[i].LastIndexOf('/');

            string newFileName = selectionArray[i].Substring(lastSlash + 1, periodIndex - lastSlash - 1);

            // Removes from the file name first.
            if (removeText.use)
                newFileName = newFileName.Replace(removeText.text, "");

            // Replaces text from the file name next.
            if (useReplacement)
                newFileName = newFileName.Replace(stringToReplace, stringToReplaceWith);

            if (baseName.use)
                newFileName = baseName.text;
            if (prefix.use)
                newFileName = prefix.text + newFileName;
            if (suffix.use)
                newFileName += suffix.text;

            if (increment)
                newFileName += (incrementStart + i * incrementValueCount).ToString().PadLeft(Mathf.Max(incrementPadding, selectionArray.Count.ToString().Length), '0');

            string newPath = selectionArray[i].Remove(lastSlash + 1, periodIndex - lastSlash - 1);
            newPath = newPath.Insert(lastSlash + 1, newFileName);

            Debug.Log("Renaming:  " + selectionArray[i] + " to " + newFileName);

            string s = AssetDatabase.RenameAsset(selectionArray[i], newFileName);
            if (!string.IsNullOrEmpty(s))
                Debug.LogError(s);
        }
    }

    public struct RenameToggle
    {
        public bool use;
        public string text;
    }
}