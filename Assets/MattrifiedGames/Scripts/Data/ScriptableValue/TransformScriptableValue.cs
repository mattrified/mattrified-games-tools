using UnityEngine;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Trasnform")]
    public class TransformScriptableValue : ScriptableValue<Transform, UnityTransformEvent>
    {
        protected override void AssignDefault()
        {
            _value = new GameObject(this.name + "' s default transform.").transform;
            assigned = true;
        }
    }

    [System.Serializable()]
    public class UnityTransformEvent : UnityEngine.Events.UnityEvent<Transform> { }
}