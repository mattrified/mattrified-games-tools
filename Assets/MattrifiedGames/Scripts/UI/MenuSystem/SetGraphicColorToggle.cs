using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGraphicColorToggle : MonoBehaviour
{
    public Graphic graphic;

    public Color offColor;
    public Color onColor;

    public void SetOnColor()
    {
        graphic.color = onColor;
    }

    public void SetOffColor()
    {
        graphic.color = offColor;
    }
}
