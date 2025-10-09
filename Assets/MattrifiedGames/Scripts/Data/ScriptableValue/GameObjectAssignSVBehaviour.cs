using UnityEngine;

namespace MattrifiedGames.SVData
{
    public class GameObjectAssignSVBehaviour : AssignScriptableValueBehaviour<GameObjectScriptableValue, GameObject, UnityGameObjectEvent>
    {
        protected override void Awake()
        {
            if (value == null)
                value = this.gameObject;

            base.Awake();
        }
    }
}