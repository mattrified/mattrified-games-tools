using UnityEngine;
using System.Collections;

namespace MattrifiedGames.Utility
{
    public class BoneVisualizer : MonoBehaviour
    {
        public void OnDrawGizmos()
        {
            Transform currentTransform = this.transform;
            while (currentTransform.childCount == 1)
            {
                Gizmos.DrawWireSphere(currentTransform.position, 0.1f);
                Gizmos.DrawLine(currentTransform.position, currentTransform.GetChild(0).position);
                currentTransform = currentTransform.GetChild(0);
            }
            Gizmos.DrawWireSphere(currentTransform.position, 0.05f);
        }
    }
}