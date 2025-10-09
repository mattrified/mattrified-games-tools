using UnityEngine;
using System.Xml.Serialization;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu()]
    public class SaveLoadScriptableObject : ScriptableObject
    {
        [SerializeField()]
        protected string fileName;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public string FileName
        {
            get
            {
                return fileName;
            }


            set
            {
                fileName = value;
            }
        }
#endif

        [SerializeField()]
        protected ScriptableValueBase[] saveData;

        [System.NonSerialized()]
        bool loaded;


        [System.NonSerialized()]
        SaveLoadSOHelper helper;

        public SaveLoadSOHelper CurrentHelper
        {
            get { return helper; }
            set
            {
                if (helper == null)
                {
                    helper = value;
                    DontDestroyOnLoad(helper.gameObject);
                }
                else
                {
                    Destroy(helper.gameObject);
                }
            }
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        public bool SetHelper(SaveLoadSOHelper newHelper)
        {
            if (newHelper == null)
                return false;

            if (helper == null)
            {
                helper = newHelper;
                DontDestroyOnLoad(newHelper.gameObject);
                return true;
            }
            else
            {
                Destroy(newHelper.gameObject);
                return false;
            }
        }
#endif


        [ContextMenu("Clear Data")]
        public virtual void ClearData()
        {
            PlayerPrefs.DeleteKey(fileName);
            PlayerPrefs.Save();
        }

        [ContextMenu("Test Save Internal")]
        protected virtual string SaveInternal()
        {
            string[] strings = new string[saveData.Length * 2];

            for (int i = 0; i < saveData.Length; i++)
            {
                strings[i * 2 + 0] = saveData[i].name;
                strings[i * 2 + 1] = saveData[i].Save();
            }

            XmlSerializer s = new XmlSerializer(typeof(string[]));

            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                s.Serialize(writer, strings);

#if UNITY_EDITOR
                Debug.Log(writer.ToString());
#endif

                return writer.ToString();
            }
        }

        [ContextMenu("Test Save")]
        public virtual void Save()
        {
            if (!loaded)
            {
                Load();
            }
            string saveData = SaveInternal();
            PlayerPrefs.SetString(fileName, saveData);
            PlayerPrefs.Save();
        }

        [ContextMenu("Test Print")]
        public virtual void Print()
        {
            string data = PlayerPrefs.GetString(fileName, string.Empty);
            //Debug.Log(data);
        }

        public void Check()
        {
            string data = PlayerPrefs.GetString(fileName, string.Empty);
            if (string.IsNullOrEmpty(data))
            {
                //Debug.Log("Data is null!");
                Save();
                Save();
                Save();
            }
            else
            {
                //Debug.Log("Load data found:  " + data);
            }
        }

        [ContextMenu("Test Load")]
        public virtual void Load()
        {
            loaded = true;
            string data = PlayerPrefs.GetString(fileName, string.Empty);
            if (string.IsNullOrEmpty(data))
            {
                //Debug.Log("Load Failed:  Resetting all data");
                for (int i = 0; i < saveData.Length; i++)
                {
                    saveData[i].Clear();
                }
                return;
            }

            Parse(data);
        }

        protected void Parse(string str)
        {
            string[] data = null;
            XmlSerializer s = new XmlSerializer(typeof(string[]));

            using (System.IO.StringReader reader = new System.IO.StringReader(str))
            {
                data = (string[])s.Deserialize(reader);
            }

            if (data == null)
                return;

            // Loads the data.  Does it this way in case order changes or data is deleated or data or renamed or something.
            for (int i = 0; i < data.Length; i += 2)
            {
                for (int j = 0; j < saveData.Length; j++)
                {
                    if (data[i + 0] == saveData[j].name)
                    {
                        //Debug.Log(data[i]);
                        //Debug.Log(data[i + 1]);
                        saveData[j].Load(data[i + 1]);
                        break;
                    }
                }
            }

            data = null;
            s = null;
        }
    }
}