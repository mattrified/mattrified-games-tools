using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.SVData
{
    public class PauseSVBehaviour : BoolSVBehaviour
    {
        [SerializeField()]
        private Behaviour[] preDefinedBehaviours;

        [SerializeField()]
        private List<Behaviour> behaviours = new List<Behaviour>();

        [SerializeField()]
        bool findBehaviours;

        protected override void Awake()
        {
            onChangeEvent.AddListener(PauseGame);
            base.Awake();
        }

        void PauseGame(bool pause)
        {
            if (pause)
            {
                // TODO:  Clean up maybe...unsure...
                behaviours.Clear();
                behaviours.AddRange(preDefinedBehaviours);
                if (findBehaviours)
                    behaviours.AddRange(GetComponentsInChildren<Behaviour>());
                behaviours.RemoveAll(IsNotEnable);
                foreach (Behaviour b in behaviours)
                    b.enabled = false;
            }
            else
            {
                foreach (Behaviour b in behaviours)
                    b.enabled = true;
                behaviours.Clear();
            }
        }

        private bool IsNotEnable(Behaviour b)
        {
            return !b.enabled;
        }
    }
}