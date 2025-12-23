using UnityEngine;

namespace MattrifiedGames.Utility
{
    [ExecuteInEditMode] // Allows preview in Editor even when not playing
    public class EasingTester : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float time = 0f;

        public bool autoPlay = true;
        public float playSpeed = 1f;

        public EasingType easingType = EasingType.QuadEaseOut;

        public enum EasingType
        {
            SineEaseIn,
            SineEaseOut,
            SineEaseInOut,

            QuadEaseIn,
            QuadEaseOut,
            QuadEaseInOut,

            CubicEaseIn,
            CubicEaseOut,
            CubicEaseInOut,

            QuartEaseIn,
            QuartEaseOut,
            QuartEaseInOut,

            QuintEaseIn,
            QuintEaseOut,
            QuintEaseInOut,

            ExpoEaseIn,
            ExpoEaseOut,
            ExpoEaseInOut,

            CircEaseIn,
            CircEaseOut,
            CircEaseInOut,

            BackEaseIn,
            BackEaseOut,
            BackEaseInOut,

            ElasticEaseIn,
            ElasticEaseOut,
            ElasticEaseInOut,

            BounceEaseIn,
            BounceEaseOut,
            BounceEaseInOut
        }

        [Header("Visual Output")]
        [ReadOnly] public float easedValue = 0f;

        private void Update()
        {
            if (autoPlay && Application.isPlaying)
            {
                time += Time.deltaTime * playSpeed;
                if (time > 1f) time -= 1f; // Loop
            }

            easedValue = ApplyEasing(easingType, time);
        }

        private float ApplyEasing(EasingType type, float t)
        {
            return type switch
            {
                EasingType.SineEaseIn => EasingsHelper.SineEaseIn(t),
                EasingType.SineEaseOut => EasingsHelper.SineEaseOut(t),
                EasingType.SineEaseInOut => EasingsHelper.SineEaseInOut(t),

                EasingType.QuadEaseIn => EasingsHelper.QuadEaseIn(t),
                EasingType.QuadEaseOut => EasingsHelper.QuadEaseOut(t),
                EasingType.QuadEaseInOut => EasingsHelper.QuadEaseInOut(t),

                EasingType.CubicEaseIn => EasingsHelper.CubicEaseIn(t),
                EasingType.CubicEaseOut => EasingsHelper.CubicEaseOut(t),
                EasingType.CubicEaseInOut => EasingsHelper.CubicEaseInOut(t),

                EasingType.QuartEaseIn => EasingsHelper.QuartEaseIn(t),
                EasingType.QuartEaseOut => EasingsHelper.QuartEaseOut(t),
                EasingType.QuartEaseInOut => EasingsHelper.QuartEaseInOut(t),

                EasingType.QuintEaseIn => EasingsHelper.QuintEaseIn(t),
                EasingType.QuintEaseOut => EasingsHelper.QuintEaseOut(t),
                EasingType.QuintEaseInOut => EasingsHelper.QuintEaseInOut(t),

                EasingType.ExpoEaseIn => EasingsHelper.ExpoEaseIn(t),
                EasingType.ExpoEaseOut => EasingsHelper.ExpoEaseOut(t),
                EasingType.ExpoEaseInOut => EasingsHelper.ExpoEaseInOut(t),

                EasingType.CircEaseIn => EasingsHelper.CircEaseIn(t),
                EasingType.CircEaseOut => EasingsHelper.CircEaseOut(t),
                EasingType.CircEaseInOut => EasingsHelper.CircEaseInOut(t),

                EasingType.BackEaseIn => EasingsHelper.BackEaseIn(t),
                EasingType.BackEaseOut => EasingsHelper.BackEaseOut(t),
                EasingType.BackEaseInOut => EasingsHelper.BackEaseInOut(t),

                EasingType.ElasticEaseIn => EasingsHelper.ElasticEaseIn(t),
                EasingType.ElasticEaseOut => EasingsHelper.ElasticEaseOut(t),
                EasingType.ElasticEaseInOut => EasingsHelper.ElasticEaseInOut(t),

                EasingType.BounceEaseIn => EasingsHelper.BounceEaseIn(t),
                EasingType.BounceEaseOut => EasingsHelper.BounceEaseOut(t),
                EasingType.BounceEaseInOut => EasingsHelper.BounceEaseInOut(t),

                _ => t // fallback (linear)
            };
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            Gizmos.color = Color.cyan;
            Vector3 start = new Vector3(0f, 0f, 0f);
            const int steps = 100;

            for (int i = 1; i <= steps; i++)
            {
                float t = i / (float)steps;
                float y = ApplyEasing(easingType, t);
                Vector3 end = new Vector3(t * 5f, y * 5f, 0f); // Scale for visibility

                Gizmos.DrawLine(start, end);
                start = end;
            }

            // Draw current position marker
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(time * 5f, easedValue * 5f, 0f), 0.15f);

            // Grid lines
            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
            for (int i = 0; i <= 5; i++)
            {
                float x = i;
                Gizmos.DrawLine(new Vector3(x, 0f, 0f), new Vector3(x, 5f, 0f));
                Gizmos.DrawLine(new Vector3(0f, x, 0f), new Vector3(5f, x, 0f));
            }
        }
#endif
    }

    // Simple attribute to make a field read-only in the Inspector
    public class ReadOnly : PropertyAttribute { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnly))]
    public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
#endif
}