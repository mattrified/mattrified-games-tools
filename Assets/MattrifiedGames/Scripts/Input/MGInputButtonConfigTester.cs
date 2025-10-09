using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MGInputButtonConfigTester : MonoBehaviour
{
    public int playerIndex;

    public Image[] imageInputs;

    private void Update()
    {
        var input = MGInputManager.playerInputs[playerIndex];
        if (input == null)
            return;

        foreach (var img in imageInputs)
            img.color = Color.gray;

        bool isKeyboard = input.device is Keyboard;
        var kList = input.mainInputSet.DirectionAndButtonList;
        for (int i = 0; i < kList.Count; i++)
        {
            if (input.TestInput(kList[i]) >= MGInputState.Pressed)
            {
                imageInputs[i].color = Color.white;
            }
        }
    }
}
