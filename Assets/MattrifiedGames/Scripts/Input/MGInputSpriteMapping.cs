using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[CreateAssetMenu()]
public class MGInputSpriteMapping : ScriptableObject
{
    public bool sort;

    [Header("Key To String")]
    public List<KeyToStringMapping> keys = new List<KeyToStringMapping>();

    [Header("Btn To String")]
    public List<ButtonToStringMapping> buttons = new List<ButtonToStringMapping>();

    public string missingString;
    const string FormatString = "<sprite name=\"{0}\">";
    public string this[Key key]
    {
        get
        {
            var result = keys.Find(x => x.key == key);
            if (result == null)
                return missingString;
            else if (string.IsNullOrEmpty(result.spriteString))
                return result.key.ToString();
            return string.Format(FormatString, result.spriteString);
        }
    }

    public string this[Key[] key]
    {
        get
        {
            if (key == null || key.Length == 0)
                return "";

            return this[key[0]];
        }
    }

    public string this[GamepadButton[] btns]
    {
        get
        {
            if (btns == null || btns.Length == 0)
                return "";

            return this[btns[0]];
        }
    }

    public string this[GamepadButton btn]
    {
        get
        {
            var result = buttons.Find(x => x.button == btn);
            if (result == null)
                return missingString;
            else if (string.IsNullOrEmpty(result.spriteString))
                return result.button.ToString();
            return string.Format(FormatString, result.spriteString);
        }
    }

#if UNITY_EDITOR
    public Texture2D keyTempTexture;
    public string keyTempPattern;

    public Texture2D btnTempTexture;
    public string btnTempPattern;

    private void OnValidate()
    {
        if (keyTempTexture != null)
        {
            var objs = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(UnityEditor.AssetDatabase.GetAssetPath(keyTempTexture));
            foreach (var obj in objs)
            {
                if (obj is Sprite)
                {
                    // We already have this sprite key so we don't add it.
                    if (keys.Exists(x => x.spriteString == obj.name))
                        continue;

                    keys.Add(new KeyToStringMapping() { key = (Key)0, spriteString = obj.name });
                }
            }

            keyTempTexture = null;
        }

        if (btnTempTexture != null)
        {
            var objs = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(UnityEditor.AssetDatabase.GetAssetPath(btnTempTexture));
            foreach (var obj in objs)
            {
                if (obj is Sprite)
                {
                    // We already have this sprite key so we don't add it.
                    if (buttons.Exists(x => x.spriteString == obj.name))
                        continue;

                    buttons.Add(new ButtonToStringMapping() { button = (GamepadButton)0, spriteString = obj.name });
                }
            }

            btnTempTexture = null;
        }

        foreach (var k in keys)
        {
            if (!string.IsNullOrEmpty(k.keyAttempt) && System.Enum.TryParse(typeof(Key), k.keyAttempt, out object result))
            {
                k.key = (Key)result;
            }

            k.label = k.key + " : " + k.spriteString;
        }

        foreach (var b in buttons)
        {
            if (!string.IsNullOrEmpty(b.btnAttempt) && System.Enum.TryParse(typeof(GamepadButton), b.btnAttempt, out object result))
            {
                b.button = (GamepadButton)result;
            }

            b.label = b.button + " : " + b.spriteString;
        }

        if (sort)
        {
            keys.Sort((x, y) =>
            {
                if (x.key == Key.None && y.key == Key.None)
                {
                    return string.Compare(x.spriteString, y.spriteString);
                }
                else
                {
                    return ((x.key == Key.None ? (Key)1000 : x.key) - (y.key == Key.None ? (Key)1000 : y.key));
                }
            });

            buttons.Sort((x, y) => (x.button == y.button) ? string.Compare(x.spriteString, y.spriteString) : x.button - y.button);
        }
            
    }
    
    [ContextMenu("Add Missing Keys")]
    void MissingKeys()
    {
        Key[] keyEnum = (Key[])System.Enum.GetValues(typeof(Key));
        foreach (var k in keyEnum)
        {
            if (keys.Exists(x => x.key == k))
                continue;
            else
            {
                keys.Add(new KeyToStringMapping() { key = k, spriteString = "MISSING: " + k.ToString() });
            }
        }
    }

    [ContextMenu("Test Key Split")]
    void TestKeyUnderscoreSplit()
    {
        Key[] keyEnum = (Key[])System.Enum.GetValues(typeof(Key));

        foreach (var key in keyEnum)
        {
            string fakeString = string.Format(keyTempPattern, key.ToString().ToLower());
            var result = keys.Find(x => x.spriteString == fakeString);
            if (result != null)
            {
                result.key = key;
            }
        }
    }

#endif

    [System.Serializable()]
    public class KeyToStringMapping
    {
#if UNITY_EDITOR
        public string label;
        public string keyAttempt;
#endif
        public Key key;
        public string spriteString;
    }

    [System.Serializable()]
    public class ButtonToStringMapping
    {
#if UNITY_EDITOR
        public string label;
        public string btnAttempt;
#endif
        // TODO:  Add platform information?
        public GamepadButton button;
        public string spriteString;
    }
}