using UnityEngine;
using System.Collections;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Game Object")]
    public class GameObjectScriptableValue : ScriptableValue<GameObject, UnityGameObjectEvent>
    {

    }

    [System.Serializable()]
    public class UnityGameObjectEvent : UnityEngine.Events.UnityEvent<GameObject> { }
}