using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData.Lists
{
    public abstract class ScriptableValueList<T> : ScriptableObject where T : Component
    {
        [SerializeField()]
        protected bool allowDuplicates;

        [System.NonSerialized()]
        protected List<T> list = new List<T>();

        public int Count
        {
            get; protected set;
        }

        public void Add(T itemToAdd)
        {
            if (!allowDuplicates && list.Contains(itemToAdd))
            {
                Debug.LogWarning("trying to add object twice.");
            }
            else
            {
                list.Add(itemToAdd);
            }
            Count = list.Count;
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public List<T> List
        {
            get
            {
                return list;
            }
        }

        public void Remove(T itemToRmove)
        {
            list.Remove(itemToRmove);
            Count = list.Count;
        }

        public void Clear()
        {
            list.Clear();
            Count = 0;
        }
    }
}