#if USING_SGF && USING_INCONTROL
using UnityEngine;
using System.Collections;
using SG.Vignettitor.VignetteData;
using UnityEngine.UI;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField()]
    DialogManagerSingleton singleton = null;

    [SerializeField()]
    SimpleDialogBehaviour simpleDialogTop = null, simpleDialogBottom = null;

    [SerializeField()]
    ChoiceDialogBehaviour choiceDialog = null;

    IDialogInput currentDialogInput = null;

    public void Awake()
    {
        singleton.instance = this;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        singleton.instance = null;
    }

    VignetteGraph graph;
    VignetteNode currentNode;

    public void StartDialogTree(VignetteGraph graph)
    {
        this.graph = graph;
        currentNode = graph.Entry;

        currentDialogInput = null;
        simpleDialogBottom.gameObject.SetActive(false);
        simpleDialogTop.gameObject.SetActive(false);
        choiceDialog.gameObject.SetActive(false);

        BeginDialog();
        gameObject.SetActive(true);
    }

    private void BeginDialog()
    {
        if (currentNode is DialogNode)
        {
            DialogNode dN = (DialogNode)currentNode;
            if (dN.displayOnTop)
            {
                simpleDialogTop.Show(dN);
                currentDialogInput = simpleDialogTop;
                choiceDialog.gameObject.SetActive(false);
            }
            else
            {
                simpleDialogBottom.Show(dN);
                currentDialogInput = simpleDialogBottom;
                choiceDialog.gameObject.SetActive(false);
            }
            return;
        }
        else if (currentNode is DialogChoiceNode)
        {
            DialogChoiceNode dnc = (DialogChoiceNode)currentNode;
            simpleDialogTop.gameObject.SetActive(false);
            currentDialogInput = choiceDialog;
            choiceDialog.Show(dnc);
        }
        else if (currentNode is SetValueNode)
        {
            SetValueNode svn = (SetValueNode)currentNode;
            svn.UpdateValue();
            NextNode(0);
        }
        else if (currentNode is TestValueNode)
        {
            TestValueNode tvn = (TestValueNode)currentNode;
            NextNode(tvn.TestValue());
        }
    }

    //public void OnInput(ActionSet set)

    internal void NextNode(int index = 0)
    {
        if (currentNode.Children.Length == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        currentNode = currentNode.Children[index];

        BeginDialog();
    }

    internal void OnAction(InControlDefinition.InControlDefinedPlayerActionSet actionSet)
    {
        if (currentDialogInput == null)
            return;

        var direction = actionSet.GetTwoAxisAction(0);

        var lastValue = direction.LastValue;
        var value = direction.Value;

        if (lastValue.x > -0.25 && value.x <= -0.25f)
            currentDialogInput.OnLeft();
        else if (lastValue.x < 0.25f && value.x >= 0.25f)
            currentDialogInput.OnRight();

        if (lastValue.y > -0.25 && value.y <= -0.25f)
        {
            Debug.Log("ON DOWN");
            currentDialogInput.OnDown();
        }
        else if (lastValue.y < 0.25f && value.y >= 0.25f)
        {
            Debug.Log("ON UP");
            currentDialogInput.OnUp();
        }

        if (actionSet.GetAction(4).WasPressed)
            currentDialogInput.OnConfirm();
    }
}
#endif