#if USING_SGF
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SimpleDialogBehaviour : MonoBehaviour, IDialogInput
{
    [SerializeField()]
    DialogManagerSingleton dialogManagerSingleton;

    [SerializeField()]
    Text speaker = null, text;

    public void Show(DialogNode node)
    {
        speaker.text = node.speakerName;
        text.text = node.dialog;
        gameObject.SetActive(true);
    }

    public void OnCancel()
    {
        
    }

    public void OnConfirm()
    {
        dialogManagerSingleton.instance.NextNode();
    }

    public void OnDown()
    {
        
    }

    public void OnLeft()
    {
        
    }

    public void OnRight()
    {
        
    }

    public void OnUp()
    {
        
    }
}
#endif