using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MattrifiedGames.SerializedDict
{
    public abstract class SerializedDictionary<T, KEY, VALUE> where T : SerializedKeyValuePair<KEY, VALUE>, new()
    {
        [SerializeField()]
        List<T> list;

        [System.NonSerialized()]
        protected KEY searchKey;

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public VALUE this[KEY key]
        {
            get
            {
                return Find(key, default(VALUE));
            }

            set
            {
                int index = FindIndex(key);
                if (index >= 0)
                    list[index].value = value;
            }
        }

        public VALUE GetByIndex(int index)
        {
            return list[index].value;
        }

        public T GetPairByIndex(int index)
        {
            return list[index];
        }

        public void SetValueByIndex(int index, VALUE value)
        {
            list[index].value = value;
        }

        public List<KEY> CreateKeyList()
        {
            List<KEY> keyList = new List<KEY>(list.Count);
            for (int i = 0, len = Count; i < len; i++)
            {
                keyList.Add(list[i].key);
            }
            return keyList;
        }

        public Dictionary<KEY, VALUE> CreateDictionary()
        {
            Dictionary<KEY, VALUE> dict = new Dictionary<KEY, VALUE>();
            for (int i = 0; i < list.Count; i++)
            {
                if (dict.ContainsKey(list[i].key))
                {
                    Debug.LogWarning(string.Format("List contains duplicates:  {0}", list[i].key));
                    continue;
                }

                dict.Add(list[i].key, list[i].value);
            }

            return dict;
        }

        public void Add(KEY key, VALUE value, bool replace)
        {
            int index = FindIndex(key);
            if (index >= 0)
            {
                if (replace)
                {
                    list[index].value = value;
                }
                return;
            }

            list.Add(new T() { key = key, value = value });
        }

        public void Remove(KEY key)
        {
            int index = FindIndex(key);

            if (index >= 0)
                list.RemoveAt(index);
        }

        public void UpdateDictionary(IDictionary<object, object> dict, bool clear = false)
        {
            if (clear)
                dict.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                if (dict.ContainsKey(list[i].key))
                {
                    Debug.LogWarning(string.Format("List contains duplicates:  {0}", list[i].key));
                    continue;
                }

                dict.Add(list[i].key, list[i].value);
            }
        }

        public void UpdateDictionary(IDictionary<KEY, VALUE> dict, bool clear = false)
        {
            if (clear)
                dict.Clear();

            for (int i = 0; i < list.Count; i++)
            {
                if (dict.ContainsKey(list[i].key))
                {
                    Debug.LogWarning(string.Format("List contains duplicates:  {0}", list[i].key));
                    continue;
                }

                dict.Add(list[i].key, list[i].value);
            }
        }

        public bool ContainsKey(KEY key)
        {
            searchKey = key;
            return list.Exists(SearchPredicate);
        }

        public VALUE Find(KEY key, VALUE defaultValue)
        {
            searchKey = key;

            var result = list.Find(SearchPredicate);
            if (result == null)
                return defaultValue;

            return result.value;
        }

        public int FindIndex(KEY key)
        {
            searchKey = key;
            return list.FindIndex(SearchPredicate);
        }

        protected virtual bool SearchPredicate(T inValue)
        {
            return searchKey.Equals(inValue.key);
        }

        public void CreateFromDictionary(Dictionary<KEY, VALUE> dict, bool clear = false)
        {
            if (list == null)
                list = new List<T>();
            else if (clear)
                list.Clear();

            foreach (KEY k in dict.Keys)
            {
                T t = new T();
                t.key = k;
                t.value = dict[k];
                list.Add(t);
            }
        }
    }

    public abstract class SerializedKeyValuePair<KEY, VALUE>
    {
        public KEY key;
        public VALUE value;
    }
}