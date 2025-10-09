using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Language")]
    public class LanguageScriptableValue : ScriptableValue<SystemLanguage, UnityLanguageEvent>
    {
        private void OnEnable()
        {
            Debug.Log("Assigning language to default:  " + Application.systemLanguage);
            defaultValue = Application.systemLanguage;
        }
    }

    [System.Serializable()]
    public class UnityLanguageEvent : UnityEvent<SystemLanguage> { }
}
