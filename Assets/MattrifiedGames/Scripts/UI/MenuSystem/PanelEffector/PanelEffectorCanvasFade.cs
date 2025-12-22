using UnityEngine;

namespace MattrifiedGames.MenuSystem
{
    public class PanelEffectorCanvasFade : PanelEffectorBase
    {
        public CanvasGroup canvas;
        public float fadeInRate = 0.25f;
        public float fadeOutRate = 0.25f;
        float fadeVelocity;

        private void Start()
        {
            bool? result = TestPanel();
            if (result == true)
                canvas.alpha = 1f;
            else if (result == false)
                canvas.alpha = 0f;
        }

        public override void OnPanelActive()
        {
            FadeCanvas(1f, fadeInRate);
        }

        public override void OnPanelInactive()
        {
            FadeCanvas(0f, fadeOutRate);
        }

        private void FadeCanvas(float alpha, float rate)
        {
            float oldA = canvas.alpha;
            float newA = Mathf.SmoothDamp(oldA, alpha, ref fadeVelocity, rate);
            if (newA != oldA)
                canvas.alpha = newA;
        }
    }
}