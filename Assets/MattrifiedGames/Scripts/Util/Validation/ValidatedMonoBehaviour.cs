using UnityEngine;

namespace MattrifiedGames
{
    /// <summary>
    /// Base class for MonoBehaviours that require validation.
    /// </summary>
    public abstract class ValidatedMonoBehaviour : MonoBehaviour
    {
        [SerializeField(), Tooltip("If true, this Behaviour is valid.")]
        protected bool _valid;

        public bool Valid { get => _valid; protected set => _valid = value; }

        public abstract void OnValidate();

        public bool TestValidity()
        {
            if (!Valid)
            {
                Debug.LogWarning($"MonoBehaviour {gameObject.name} is not valid.");
                return false;
            }

            return true;
        }
    }
}