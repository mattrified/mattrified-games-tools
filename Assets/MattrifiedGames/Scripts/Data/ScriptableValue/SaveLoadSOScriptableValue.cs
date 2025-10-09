using UnityEngine;
using System.Collections;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Save Load")]
    public class SaveLoadSOScriptableValue : ScriptableValue<SaveLoadScriptableObject, UnitySaveLoadSOEvent>
    {
        public void LoadData()
        {
            if (Value != null)
                Value.Load();
        }

        public void SaveData()
        {
            if (Value != null)
                Value.Save();
        }

        public void SetValueAndLoad(SaveLoadScriptableObject slso)
        {
            Value = slso;
            LoadData();
        }
    }

    [System.Serializable()]
    public class UnitySaveLoadSOEvent : UnityEngine.Events.UnityEvent<SaveLoadScriptableObject> { }
}