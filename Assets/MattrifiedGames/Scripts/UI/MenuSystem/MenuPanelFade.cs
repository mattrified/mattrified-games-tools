using UnityEngine;

namespace MattrifiedGames.MenuSystem
{
    public class MenuPanelFade : MenuPanel
    {
        [SerializeField()]
        CanvasGroup canvas;

        [SerializeField()]
        float alpha = 0f;

        public float rate = 2f;

        bool? fading;

        private void Awake()
        {
            canvas.alpha = alpha;
        }

        public override void Open(MenuSystemBase menu)
        {
            fading = true;
            Locked = true;
            alpha = Mathf.Clamp01(alpha);
            base.Open(menu);
        }

        public override void Close(MenuSystemBase menu)
        {
            fading = false;
            Locked = true;
            alpha = Mathf.Clamp01(alpha);
            base.Close(menu);
        }

        private void Update()
        {
            if (!fading.HasValue)
            {
                return;
            }
            else if (fading.Value)
            {
                alpha += Time.deltaTime * rate;
                if (alpha >= 1f)
                {
                    Locked = false;
                    fading = null;
                }
            }
            else
            {
                alpha -= Time.deltaTime * rate;
                if (alpha <= 0)
                {
                    Locked = false;
                    fading = null;
                }
            }

            canvas.alpha = alpha;
        }
    }
}