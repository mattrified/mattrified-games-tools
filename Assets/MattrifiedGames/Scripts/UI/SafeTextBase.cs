using TMPro;
using UnityEngine.UI;

namespace Assets.MattrifiedGames.Scripts.UI
{
    public static class SafeTextBase
    {
        private static readonly string[] numbers = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "" };

        public static void DisplayValue(int value, TextMeshProUGUI[] textArray)
        {
            for (int i = 0; i < textArray.Length; i++)
            {
                if (value > 0)
                    textArray[i].text = numbers[value % 10];
                else
                    textArray[i].text = numbers[10];

                value /= 10;
            }
        }

        public static void DisplayValue(int value, Text[] textArray)
        {
            for (int i = 0; i < textArray.Length; i++)
            {
                if (value > 0)
                    textArray[i].text = numbers[value % 10];
                else
                    textArray[i].text = numbers[10];

                value /= 10;
            }
        }
    }
}
