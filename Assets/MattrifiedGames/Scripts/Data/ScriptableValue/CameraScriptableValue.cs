using UnityEngine;
using System.Collections;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Camera")]
    public class CameraScriptableValue : ScriptableValue<Camera, UnityCameraEvent>
    {

    }

    [System.Serializable()]
    public class UnityCameraEvent : UnityEngine.Events.UnityEvent<Camera> { }
}