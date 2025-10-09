using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    public class ColorSVBehaviour : ScriptableValueBehaviour<ColorScriptableValue, Color, UnityColorEvent>
    {
        public bool assignOnAwake;
        public Color initialColor;

        protected override void Awake()
        {
            base.Awake();

            if (assignOnAwake)
                scriptableValue.Value = initialColor;
        }
    }
}