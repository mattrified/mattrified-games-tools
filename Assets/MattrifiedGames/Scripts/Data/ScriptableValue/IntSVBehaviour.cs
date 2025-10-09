using UnityEngine.UI;

namespace MattrifiedGames.SVData
{
    public class IntSVBehaviour : ScriptableValueBehaviour<IntScriptableValue, int, UnityIntEvent>
    {
        public void SetText(string format)
        {
            var text = GetComponent<Text>();
            if (text == null)
                return;

            text.text = string.Format(format, scriptableValue.Value);
        }
    }
}