using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.SVData.Arrays
{
    [CreateAssetMenu(menuName ="Scriptable Arrays/String")]
    public class ScriptableStringArray : ScriptableArrayBase<string>
    {
        public override string GetItemString(int index)
        {
            return values[index];
        }
    }
}