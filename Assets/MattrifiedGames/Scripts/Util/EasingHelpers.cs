using UnityEngine;

namespace MattrifiedGames
{
    public static class EasingsHelper
    {
        /// <summary>
        /// All easing functions from https://easings.net/
        /// </summary>

        // Sine
        public static float SineEaseIn(float t) => 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
        public static float SineEaseOut(float t) => Mathf.Sin(t * Mathf.PI * 0.5f);
        public static float SineEaseInOut(float t) => -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;

        // Quad
        public static float QuadEaseIn(float t) => t * t;
        public static float QuadEaseOut(float t) => 1f - (1f - t) * (1f - t);
        public static float QuadEaseInOut(float t) => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;

        // Cubic
        public static float CubicEaseIn(float t) => t * t * t;
        public static float CubicEaseOut(float t) => 1f - Mathf.Pow(1f - t, 3f);
        public static float CubicEaseInOut(float t) => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;

        // Quart
        public static float QuartEaseIn(float t) => t * t * t * t;
        public static float QuartEaseOut(float t) => 1f - Mathf.Pow(1f - t, 4f);
        public static float QuartEaseInOut(float t) => t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;

        // Quint
        public static float QuintEaseIn(float t) => t * t * t * t * t;
        public static float QuintEaseOut(float t) => 1f - Mathf.Pow(1f - t, 5f);
        public static float QuintEaseInOut(float t) => t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) * 0.5f;

        // Expo
        public static float ExpoEaseIn(float t) => t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
        public static float ExpoEaseOut(float t) => t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        public static float ExpoEaseInOut(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return t < 0.5f
                ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f
                : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;
        }

        // Circ
        public static float CircEaseIn(float t) => 1f - Mathf.Sqrt(1f - t * t);
        public static float CircEaseOut(float t) => Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
        public static float CircEaseInOut(float t) => t < 0.5f
            ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * t, 2f))) * 0.5f
            : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) * 0.5f;

        // Back
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;
        private const float c3 = c1 + 1f; // 2.70158f

        public static float BackEaseIn(float t) => c3 * t * t * t - c1 * t * t;
        public static float BackEaseOut(float t) => 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        public static float BackEaseInOut(float t) => t < 0.5f
            ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) * 0.5f
            : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) * 0.5f;

        // Elastic
        private const float c4 = (2f * Mathf.PI) / 3f;
        private const float c5 = (2f * Mathf.PI) / 4.5f;

        public static float ElasticEaseIn(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
        }

        public static float ElasticEaseOut(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        public static float ElasticEaseInOut(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return t < 0.5f
                ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) * 0.5f
                : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * c5)) * 0.5f + 1f;
        }

        // Bounce
        private const float n1 = 7.5625f;
        private const float d1 = 2.75f;

        private static float BounceOut(float t)
        {
            if (t < 1f / d1) return n1 * t * t;
            if (t < 2f / d1) return n1 * (t -= 1.5f / d1) * t + 0.75f;
            if (t < 2.5f / d1) return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }

        public static float BounceEaseIn(float t) => 1f - BounceOut(1f - t);
        public static float BounceEaseOut(float t) => BounceOut(t);
        public static float BounceEaseInOut(float t) => t < 0.5f
            ? (1f - BounceOut(1f - 2f * t)) * 0.5f
            : (1f + BounceOut(2f * t - 1f)) * 0.5f;
    }
}