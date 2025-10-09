using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if USING_TEXTMESHPRO
using TMPro;
#endif

public class TextTMProHelper : MonoBehaviour
{
#if USING_TEXTMESHPRO
    [SerializeField()]
    TextMeshProUGUI text;
#else
    [SerializeField()]
    Text text;
#endif

    public void SetText(string value)
    {
        text.text = value;
    }

    public void SetAlpha(float alpha)
    {
        var color = text.color;
        color.a = alpha;
        text.color = color;
    }

    public void SetColorFromHTMLString(string htmlString)
    {
        Color c = text.color;
        if (ColorUtility.TryParseHtmlString(htmlString, out c))
            text.color = c;
    }

#if USING_TEXTMESHPRO
    [SerializeField()]
    public TextMeshProUGUI TextObject
    {
        get
        {
            return text;
        }
    }
#else
    public Text TextObject
    {
        get
        {
            return text;
        }
    }
#endif
}
