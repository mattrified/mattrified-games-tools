using UnityEngine;
using System.Collections;

namespace MattrifiedGames.SVData
{
    public abstract class ScriptableValueBase : ScriptableObject
    {
        public virtual string Save() { return string.Empty; }
        public virtual void Load(string s) { }
        public abstract void Clear();
    }
}