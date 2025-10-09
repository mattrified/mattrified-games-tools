using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Mattrified Games/Layered Name Generator")]
public class LayeredNameGenerator : ScriptableObject
{
    [System.Serializable()]
    public class Layer
    {
        public string[] names;

        public string this[int i] => names[i];
    }

    public Layer[] layers;

    public int digitCount = 4;

    public string GenerateName()
    {
        string s = string.Empty;
        for (int i = 0; i < layers.Length; i++)
        {
            s += layers[i][Random.Range(0, layers[i].names.Length)] + " ";
        }

        for (int i = 0; i < digitCount; i++)
        {
            s += Random.Range(0, 10).ToString();
        }

        Debug.Log("Generated:  " + s);

        return s;
    }
}
