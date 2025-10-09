using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MGInputGameSpriteUpdater : MonoBehaviour
{
    public TextMeshProUGUI text;

    [TextArea()]
    public string textToMap;

    public bool pullFromText;

    public MGInputGameSpriteMapping mapping;

    public int playerIndex;

    public MattrifiedGames.EventSO.IntEventSO OnInputAssigned;

    private void OnValidate()
    {
        if (text && pullFromText)
            textToMap = text.text;
    }

    private void OnEnable()
    {
        OnInputAssigned?.AddListener(UpdateInput);
        UpdateInput(playerIndex);
    }

    private void OnDisable()
    {
        OnInputAssigned?.RemoveListener(UpdateInput);
    }

    public void UpdateInput(int index)
    {
        if (index != playerIndex)
            return;

        if (MGInputManager.playerInputs[playerIndex] == null)
        {
            text.text = string.Empty;
            return;
        }

        text.text = mapping.UpdateText(MGInputManager.playerInputs[playerIndex], textToMap);
    }
}
