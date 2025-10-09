using UnityEngine;
using System.Collections;

public interface IDialogInput
{
    void OnUp();
    void OnDown();
    void OnLeft();
    void OnRight();

    void OnConfirm();
    void OnCancel();
}
