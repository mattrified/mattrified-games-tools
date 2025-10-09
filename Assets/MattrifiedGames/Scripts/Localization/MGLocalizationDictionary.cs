using MattrifiedGames.SVData;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MGLocalizationDictionary : ScriptableObject
{
    [SerializeField(), NonReorderable()]
    private List<LocalizedStringEntry> localizedStrings;

    [SerializeField()]
    private Dictionary<string, LocalizedStringEntry> entryDict;

    [System.Serializable()]
    public class LocalizedStringEntry
    {
        public string key;

        [NonReorderable()]
        public List<LocalizedItem> languageList;
    }

    [System.Serializable()]
    public class LocalizedItem
    {
        public SystemLanguage language;
        public string value;
    }

    internal bool HasKey(string key)
    {
        return localizedStrings.Exists(x => x.key.Equals(key));
    }

    public string[] KeyArray
    {
        get
        {
            string[] s = new string[localizedStrings.Count];
            for (int i = 0; i < s.Length; i++)
                s[i] = localizedStrings[i].key;
            return s;
        }
    }

    [ContextMenu("Print JSon")]
    public void ToJson()
    {
        string s = JsonUtility.ToJson(this, true);
        Debug.Log(s);
    }

    /// <summary>
    /// Returns the string based on the designated language result
    /// </summary>
    /// <param name="key"></param>
    /// <param name="language"></param>
    /// <param name="defaultLang"></param>
    /// <returns></returns>
    public string GetValue(string key, SystemLanguage language, SystemLanguage defaultLang = SystemLanguage.English)
    {
        if (entryDict == null)
        {
            entryDict = new Dictionary<string, LocalizedStringEntry>();
            for (int i = 0, len = localizedStrings.Count; i < len; i++)
            {
                entryDict.Add(localizedStrings[i].key, localizedStrings[i]);
            }
        }

        if (entryDict.TryGetValue(key, out var result))
        {
            var langResult = result.languageList.Find(x => x.language == language);

            if (langResult == null)
            {
                if (defaultLang != SystemLanguage.Unknown)
                {
                    langResult = result.languageList.Find(x => x.language == defaultLang);
                    if (langResult == null)
                    {
                        return string.Format("No value found for {2} or {3} with Key {0} in dict {1}", key, this.name, language, defaultLang);
                    }
                    else
                    {
                        return langResult.value;
                    }
                }
                else
                {
                    return string.Format("No value found for {2} with Key {0} in dict {1}", key, this.name, language);
                }
            }
            else
            {
                return langResult.value;
            }
        }
        else
        {
            return string.Format("Key {0} has no value in dict {1}", key, this.name);
        }
    }

    protected internal void AddKey(string key, string defaultText)
    {
        if (HasKey(key))
            return;

        localizedStrings.Add(
            new LocalizedStringEntry()
            {
                key = key,
                languageList = new List<LocalizedItem>()
                {
                    new LocalizedItem()
                    {
                         language = SystemLanguage.English,
                          value = defaultText,
                    }
                }
            }
        );
    }
}