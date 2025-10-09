using UnityEngine;
using System.Collections;

namespace MattrifiedGames.HairKit
{
    [ExecuteInEditMode()]
    public class HairKitLinePoint : MonoBehaviour
    {
        public Vector3 lockPosition;
        public bool locked;

        private void LateUpdate()
        {
            if (!locked)
                lockPosition = transform.position;
            else
                transform.position = lockPosition;
        }
    }
}