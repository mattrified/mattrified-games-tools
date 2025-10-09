using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldToUIInfo : MonoBehaviour
{
    public GameObjectScriptableValue worldToUIGameObjectScriptableValue;
    public Transform trackedTransform;

    public abstract void Assign();
    public abstract void Unassign();
}