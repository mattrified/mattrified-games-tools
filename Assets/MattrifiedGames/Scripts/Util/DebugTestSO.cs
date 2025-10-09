using UnityEngine;
using System.Collections.Generic;
using MattrifiedGames.SVData;

namespace MattrifiedGames.Utility
{
    [CreateAssetMenu()]
    public class DebugTestSO : ScriptableObject
    {
        public SaveLoadSOScriptableValue data;

#if UNITY_EDITOR
        public List<UnityEditor.BuildTargetGroup> buildTargetGroups = new List<UnityEditor.BuildTargetGroup>();
        public List<UnityEditor.BuildTarget> buildTargets = new List<UnityEditor.BuildTarget>();
#endif

        // Loads the data.
        [ContextMenu("Load Data")]
        public void LoadData()
        {
            data.LoadData();
        }

        public void LogMessage(object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.Log(message);
#endif
        }

        public void LogWarning(object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogWarning(message);
#endif
        }

        public void LogError(object message)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug.LogError(message);
#endif
        }

        public void LogString(string message)
        {
            LogMessage(message);
        }

        public void LogStringWarning(string message)
        {
            LogWarning(message);
        }

        public void LogStringError(string message)
        {
            LogError(message);
        }
    }
}