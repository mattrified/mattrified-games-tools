#if USING_SGF
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChoiceDialogBehaviour : MonoBehaviour, IDialogInput
{
    [SerializeField()]
    DialogManagerSingleton dialogManagerSingleton;

    [SerializeField()]
    Text[] choices = null;

    [SerializeField()]
    Transform arrow;

    int index = 0;
    int length;

    public void Show(DialogChoiceNode node)
    {
        index = 0;
        length = node.choices.Count;

        for (int i = 0, len = choices.Length; i < len;  i++)
        {
            if (i < node.choices.Count)
            {
                choices[i].gameObject.SetActive(true);
                choices[i].text = node.choices[i];
            }
            else
            {
                choices[i].gameObject.SetActive(false);
            }
        }

        gameObject.SetActive(true);

        pos = arrow.localPosition;
        pos.y = choices[index].transform.localPosition.y;
    }

    Vector3 pos;

    void Update()
    {
        pos.y = Mathf.SmoothStep(pos.y, choices[index].transform.localPosition.y, 10f * Time.deltaTime);
        arrow.localPosition = pos;
    }

    public void OnCancel()
    {
        
    }

    public void OnConfirm()
    {
        dialogManagerSingleton.instance.NextNode(index);
    }

    public void OnDown()
    {
        Debug.Log("ON DOWN");
        index++;
        if (index >= length)
            index = 0;
    }

    public void OnLeft()
    {
        
    }

    public void OnRight()
    {
        
    }

    public void OnUp()
    {
        index--;
        if (index < 0)
            index = length - 1;
    }
}
#endif