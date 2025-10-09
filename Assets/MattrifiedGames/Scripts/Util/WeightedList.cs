using UnityEngine;

namespace MattrifiedGames.Utility
{

    public abstract class WeightedList<T, U> where T : WeightedItem<U>
    {
        [SerializeField()]
        protected T[] list;

        [System.NonSerialized()]
        protected bool initialized;

        [System.NonSerialized()]
        protected int weight = 0;

        public U GetItem()
        {
            if (!initialized)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    weight += list[i].weight;
                }
                initialized = true;
            }

            float w = weight;
            for (int i = 0, len = list.Length; i < len; i++)
            {
                if (Random.value * weight < list[i].weight)
                {
                    return list[i].value;
                }
                w -= list[i].weight;
            }
            return list[list.Length - 1].value;
        }

        /*public U GetItemFP(LockstepRandomization random, ref int randomIndex)
        {
            if (!initialized)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    weight += list[i].weight;
                }
                initialized = true;
            }

            float w = weight;
            for (int i = 0, len = list.Length; i < len; i++)
            {
                if (random.GetRandom(ref randomIndex) * weight < list[i].weight)
                {
                    return list[i].value;
                }
                w -= list[i].weight;
            }
            return list[list.Length - 1].value;
        }*/
    }

    public abstract class WeightedItem<T>
    {
        public int weight;
        public T value;
    }
}