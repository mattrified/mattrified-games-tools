using MattrifiedGames.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonScriptableValue<T> : ScriptableObject where T : MonoBehaviour
{
    [SerializeField()]
    bool createNewOnRequest;

    [SerializeField()]
    bool dontDestroyOnLoad;

    [SerializeField()]
    T prefab;

    [System.NonSerialized()]
    protected bool assigned = false;

    [System.NonSerialized()]
    protected T instance = null;

    public T Instance
    {
        get
        {
            if (!assigned)
            {
                if (createNewOnRequest)
                {
                    if ((object)prefab != null)
                    {
                        instance = Instantiate(prefab);
                    }
                    else
                    {
                        GameObject go = new GameObject(this.name + " Instance");
                        instance = go.AddComponent<T>();
                    }

                    if (dontDestroyOnLoad)
                        DontDestroyOnLoad(instance.gameObject);

                    OnDestroyEventBehaviour onDestroy = instance.GetComponent<OnDestroyEventBehaviour>();
                    if (onDestroy == null)
                    {
                        onDestroy = instance.gameObject.AddComponent<OnDestroyEventBehaviour>();
                        onDestroy.onDestroyEvent.AddListener(() => { instance = null; assigned = false; });
                    }

                    assigned = true;
                }
            }
            return instance;
        }

        set
        {
            Assign(value);
        }
    }

    public void ClearInstance()
    {
        instance = null;
        assigned = false;
    }

    public void Assign(T val, bool forceReplacement = false)
    {
        if (assigned)
        {

            if (forceReplacement)
            {
                instance = val;
                Debug.LogWarning(this.name + " already has a value assigned but is being forcibly replaced.");
            }
            else
            {
                Debug.LogWarning(this.name + " already has a value assigned and is not being replaced.");
            }
        }
        else
        {
            instance = val;
            assigned = true;
        }
    }
}