using UnityEngine;

namespace MattrifiedGames.MenuSystem
{
    public class MenuPanelAttractor : MonoBehaviour
    {
        public Transform t;

        public MenuPanel panel;

        public MenuElementBase element;
        public Transform otherTransform;

        Vector3 vel;
        public float smoothTime;

        // Start is called before the first frame update
        void Start()
        {
            element = panel.ActiveElement;
            otherTransform = element.transform;
            t.position = otherTransform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (element.GetInstanceID() != panel.ActiveElement.GetInstanceID())
            {
                otherTransform = panel.ActiveElement.transform;
                element = panel.ActiveElement;
            }

            t.position = Vector3.SmoothDamp(t.position, otherTransform.position, ref vel, smoothTime);

        }
    }
}