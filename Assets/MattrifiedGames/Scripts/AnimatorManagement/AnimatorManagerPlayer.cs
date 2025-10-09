using UnityEngine;

namespace MattrifiedGames.ManagedAnimation
{
    public class AnimatorManagerPlayer : MonoBehaviour
    {
        protected const float DELTA = 1f / 60f;

        public AnimatorManager manager;

        public int currentIndex;

        public float currentTime;

        public float currentTransition;

        public float animSpeed = 1f;

        public int currentFrame;
        public int previousFrame;

        // TODO:  Use an array of parameters or set the actual animator parameters of conditions to test various conditions and whatnot.

        public virtual void Awake()
        {
            if (manager == null)
                manager = GetComponent<AnimatorManager>();

            var behaviours = manager.data[currentIndex].amStateBehaviours;
            foreach (var behaviour in behaviours)
            {
                behaviour.OnManagedEnter(this);
            }
        }

        protected virtual void FixedUpdate()
        {
            currentTime += animSpeed;
            currentFrame = Mathf.FloorToInt(currentTime);

            manager.UpdateAnimator(currentIndex, currentTime * DELTA, currentTransition);

            var behaviours = manager.data[currentIndex].amStateBehaviours;
            foreach (var behaviour in behaviours)
            {
                if (behaviour.OnManagedUpdate(this))
                    break;
            }

            previousFrame = currentFrame;
        }

        public void GoToFrame(int frame)
        {
            currentTime = frame;
            currentFrame = frame;
            previousFrame = frame;
            manager.UpdateAnimator(currentIndex, currentTime * DELTA);
        }

        public void GoToState(int stateIndex, int startFrame, float transition = 0.2f)
        {
            var behaviours = manager.data[currentIndex].amStateBehaviours;
            foreach (var behaviour in behaviours)
            {
                behaviour.OnManagedExit(this);
            }

            currentIndex = stateIndex;

            currentTime = startFrame;
            currentFrame = startFrame;
            previousFrame = startFrame;

            currentTransition = transition;

            behaviours = manager.data[currentIndex].amStateBehaviours;
            foreach (var behaviour in behaviours)
            {
                behaviour.OnManagedEnter(this);
            }
        }

        public void Snap(int index, float time)
        {
            currentIndex = index;
            currentTime = time;
            currentFrame = Mathf.FloorToInt(currentTime);

            manager.UpdateAnimator(currentIndex, currentTime * DELTA, 0f);

            previousFrame = currentFrame;
        }
    }
}