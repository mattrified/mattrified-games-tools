using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.SVData
{


    public class SaveLoadSOHelper : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        string oldName;

        [SerializeField()]
        SaveLoadScriptableObject slData;

        [SerializeField()]
        int limit = 10;

        int item = 0;

        bool isHelper;

        private void Awake()
        {
            if (slData.SetHelper(this))
            {
                isHelper = true;
                oldName = slData.FileName;
            }

        }

        private void OnDestroy()
        {
            if (isHelper)
                slData.FileName = oldName;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                item++;
                int index = item % limit;

                if (index == 0)
                    slData.FileName = oldName;
                else
                    slData.FileName = oldName + (index).ToString();

                slData.Load();
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                slData.ClearData();
                slData.Load();
            }
        }
#endif
    }
}