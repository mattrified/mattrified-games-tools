using UnityEngine;
using System.Collections;

namespace MattrifiedGames.SerializedDict
{
    [System.Serializable()]
    public class SerializedStringStringDictionary : SerializedDictionary<SerializedStringStringValuePair, string, string>
    {

    }

    [System.Serializable()]
    public class SerializedStringStringValuePair : SerializedKeyValuePair<string, string> { }

    [System.Serializable()]
    public class SerializedStringIntDictionary : SerializedDictionary<SerializedStringIntValuePair, string, int>
    {

    }

    [System.Serializable()]
    public class SerializedStringIntValuePair : SerializedKeyValuePair<string, int> { }
}