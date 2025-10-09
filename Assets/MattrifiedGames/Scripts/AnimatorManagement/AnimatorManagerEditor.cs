using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.ManagedAnimation
{
    /// <summary>
    /// A behaviour that creates an editor for editing and viewing managed animation data.
    /// </summary>
    public class AnimatorManagerEditor : MonoBehaviour
    {
#if UNITY_EDITOR

        public bool hideGizmos = false;

        public const float FRAME = 1f / 60f;

        public AnimatorManager manager;

        public Material hitSphereMaterial;

        /// <summary>
        /// The index of the current animation playing
        /// </summary>
        public int index = 0;

        /// <summary>
        /// The time, in seconds, of the animation being displayed.
        /// </summary>
        public float time = 0;

        /// <summary>
        /// THe name of all the states within the animator manager
        /// </summary>
        public string[] stateNames;

        /// <summary>
        /// The previous location position of the animation.
        /// </summary>
        Vector3 previousLocalPos;

        /// <summary>
        /// The previous local rotation of the animation.
        /// </summary>
        Quaternion previousLocalRot;

        /// <summary>
        /// Used for creating bounding boxes.
        /// </summary>
        public Vector3? positionOnMouseDown = null, positionOnMouseUp = null;

        /// <summary>
        /// An array of scenes that can be switch to by the editor.
        /// </summary>
        public string[] otherScenesToLoad;

        /// <summary>
        /// Is an animation currently playing?
        /// </summary>
        public bool IsPlaying
        {
            get;
            protected set;
        }

        /// <summary>
        /// A hashtable that any class can use if needed while editing.
        /// </summary>
        public Hashtable hashtable = new Hashtable();

        /// <summary>
        /// Gets the length of the current state
        /// </summary>
        public float CurrentStateLength
        {
            get
            {
                return manager.data.GetStateLength(index);
            }
        }

        /// <summary>
        /// Gets the current frame of the animation.  Game is measured at 60 FPS.
        /// </summary>
        public int Frame
        {
            get
            {
                return Mathf.FloorToInt(time * 60f);
            }
        }

        private void OnEnable()
        {
            if (manager == null)
            {
                Debug.LogError("No Animator Manager Assigned.");
                enabled = false;
                return;
            }
        }

        protected void Start()
        {
            // Checks to see if there is a manager.
            if (manager == null)
            {
                Debug.LogError("No Animator Manager Assigned.");
                enabled = false;
                return;
            }

            // Gets the number of states
            stateNames = new string[manager.data.states.Count];
            for (int i = 0; i < stateNames.Length; i++)
            {
                stateNames[i] = manager.data[i].animationName.Name;
            }
            
            // Opens the editor
            OpenEditor();

            // Gets the animator and sets root motion to true.  We want root motion to capture delta movement.
            Animator anim = manager.GetComponent<Animator>();
            anim.applyRootMotion = true;
        }

        private void Update()
        {
            // If an animation is playing, we don't update.
            if (IsPlaying)
                return;

            /*Keyboard keyboard = null;

            foreach (var device in InputSystem.devices)
            {
                if (device is Keyboard)
                {
                    keyboard = device as Keyboard;
                }
            }*/

            //if (keyboard != null)
            //{

            // Moves the camera around.
            if (Input.GetKey(KeyCode.LeftArrow))
                Camera.main.transform.position += Vector3.left * Time.deltaTime;

            if (Input.GetKey(KeyCode.RightArrow))
                Camera.main.transform.position += Vector3.right * Time.deltaTime;

            if (Input.GetKey(KeyCode.UpArrow))
                Camera.main.transform.position += Vector3.up * Time.deltaTime;

            if (Input.GetKey(KeyCode.DownArrow))
                Camera.main.transform.position += Vector3.down * Time.deltaTime;
            //}

            // On right-mouse press, we recording the starting and ending positions to create bounding boxes.
            /*if (!positionOnMouseDown.HasValue)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    var pos = Input.mousePosition;
                    pos.z = 5f;
                    positionOnMouseDown = pos;
                }
            }
            else if (!positionOnMouseUp.HasValue)
            {
                var mP = Input.mousePosition;
                mP.z = 5f;

                var start = Camera.main.ScreenToWorldPoint(positionOnMouseDown.Value);
                var end = Camera.main.ScreenToWorldPoint(mP);

                var upperLeft = start;
                var upperRight = new Vector3(end.x, start.y, start.z);
                var lowerLeft = new Vector3(start.x, end.y, start.z);
                var lowerRight = end;

                // Draws lines to show where the boounding box will be drawn.
                Debug.DrawLine(upperLeft, upperRight, Color.blue);
                Debug.DrawLine(upperLeft, lowerLeft, Color.blue);
                Debug.DrawLine(lowerRight, upperRight, Color.blue);
                Debug.DrawLine(lowerRight, lowerLeft, Color.blue);

                if (Input.GetMouseButtonUp(1))
                {
                    var uPos = Input.mousePosition;
                    uPos.z = 5f;
                    positionOnMouseUp = uPos;

                    positionOnMouseDown = upperLeft;
                    positionOnMouseUp = lowerRight;

                    // Create box from mouse positions
                    ProcessHitBoxReagion();

                    // Resets the mouse down and up positions
                    positionOnMouseDown = null;
                    positionOnMouseUp = null;
                }
            }*/

            manager.data[index].DrawSceneEditorInfo(this);
            /*for (int i = 0; i < manager.data[index].amStateBehaviours.Count; i++)
            {
                manager.data[index].amStateBehaviours[i].DrawGizmo(this);
            }*/
        }

        /// <summary>
        /// Process the hit box region.
        /// </summary>
        private void ProcessHitBoxReagion()
        {
            Vector3 min = positionOnMouseDown.Value - transform.position;
            Vector3 max = positionOnMouseUp.Value - transform.position;

            Bounds b = new Bounds();
            b.SetMinMax(min, max);

            Debug.DrawLine(b.min, b.max, Color.gray, 2f);

            manager.data[index].ProcessNewHitBox(Frame, b);
        }

        /// <summary>
        /// Resets the character's position and time.
        /// </summary>
        public void ResetChar()
        {
            Animator anim = manager.GetComponent<Animator>();
            anim.transform.localPosition = Vector3.zero;
            anim.transform.localRotation = Quaternion.identity;
            this.time = 0;
        }

        /// <summary>
        /// Updates the animator.
        /// </summary>
        public void UpdateAnimator()
        {
            manager.UpdateAnimator(index, time, 0);
        }

        /// <summary>
        /// Players the animation.
        /// </summary>
        public void PlayAnimation()
        {
            IsPlaying = true;
            StartCoroutine(PlayAnimationRoutine());
        }

        /// <summary>
        /// Players all the animations.
        /// </summary>
        public void PlayAllAnimations()
        {
            IsPlaying = true;
            StartCoroutine(PlayAllAnimationRoutine());
        }

        /// <summary>
        /// Coroutine for playing all of a character's animations.
        /// </summary>
        protected IEnumerator PlayAllAnimationRoutine()
        {
            for (int i = 0; i < manager.data.Count; i++)
            {
                index = i;
                IsPlaying = true;
                yield return StartCoroutine(PlayAnimationRoutine());
                IsPlaying = true;
                yield return 0;
            }

            IsPlaying = false;
        }

        /// <summary>
        /// Coroutine for playing all of a character's animations.
        /// </summary>
        protected virtual IEnumerator PlayAnimationRoutine()
        {
            Animator anim = manager.GetComponent<Animator>();

            List<Vector3> deltaList = new List<Vector3>();
            List<Vector3> eulerList = new List<Vector3>();

            anim.transform.localPosition = Vector3.zero;
            anim.transform.localRotation = Quaternion.identity;

            time = 0;
            manager.UpdateAnimator(index, time, 0);

            yield return new WaitForEndOfFrame();

            manager.data[index].BeginPlaythroughProcessing(this);

            while (time < manager.data.GetStateLength(index))
            {
                manager.data[index].ProcessPreFrame(this);

                Vector3 prePos = anim.transform.localPosition;
                Vector3 preForward = anim.transform.forward;

                time += FRAME;
                manager.UpdateAnimator(index, time, 0);

                yield return new WaitForEndOfFrame();

                Quaternion newQuat = Quaternion.FromToRotation(preForward, anim.transform.forward);
                preForward = anim.transform.forward;
                Vector3 v = newQuat.eulerAngles;
                if (v.y > 180)
                    v.y = v.y - 360f;
                eulerList.Add(new Vector3(0, v.y, 0));

                Vector3 diff = anim.transform.localPosition - prePos;
                deltaList.Add(diff);

                manager.data[index].ProcessPostFrame(this);
            }

            deltaList.RemoveAt(deltaList.Count - 1);
            manager.data[index].ProcessDeltaPosList(deltaList);

            eulerList.RemoveAt(eulerList.Count - 1);
            manager.data[index].ProcessDeltaEulerList(eulerList);

            manager.data[index].EndPlaythroughProcessing(this);

            IsPlaying = false;
        }

        protected virtual void OpenEditor()
        {
            AnimatorManagerEditorWindow window = UnityEditor.EditorWindow.GetWindow<AnimatorManagerEditorWindow>();
            window.ame = this;
        }

        internal void OnSceneGUI()
        {
            if (!Application.isPlaying)
                return;

            manager.data[index].DrawSceneGUI(this);
            for (int i = 0; i < manager.data[index].amStateBehaviours.Count; i++)
            {
                manager.data[index].amStateBehaviours[i].DrawSceneGUI(this);
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || hideGizmos)
                return;

            manager.data[index].DrawGizmos(this);
            for (int i = 0; i < manager.data[index].amStateBehaviours.Count; i++)
            {
                manager.data[index].amStateBehaviours[i].DrawGizmo(this);
            }
        }
#endif
    }
}
