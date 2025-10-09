using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MattrifiedGames.SVData.Lists;

[CreateAssetMenu(menuName = "Lists/In Game Curve")]
public class InGameCurveList : ScriptableValueList<InGameCurveBase>
{
    public bool FindCurveByID(int ID, out InGameCurveBase curve)
    {
        curve = null;
        for (int i = 0; i < Count; i++)
        {
            if (list[i].curveID == ID)
            {
                curve = list[i];
                return true;
            }
        }
        return false;
    }
}

[System.Serializable()]
public class UnityInGameCurveEvent : UnityEvent<InGameCurveBase> { }