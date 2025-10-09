using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MattrifiedGames.Utility
{
    public class SetTextHelper : MonoBehaviour
    {
        public Text unityText;
        public TextMeshProUGUI tmpText;

        private void OnValidate()
        {
            unityText = GetComponent<Text>();
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        public void SetTextFromFloat(float value)
        {
            if (unityText != null)
                unityText.text = value.ToString();

            tmpText?.SetText(value.ToString());
        }
    }
}
