#if USING_SGF
using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName ="Singletons/Dialog Manager")]
public class DialogManagerSingleton : ScriptableObject
{
    [System.NonSerialized()]
    public DialogManager instance;
}
#endif
