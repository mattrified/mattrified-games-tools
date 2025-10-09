using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.EventSO
{
    [CreateAssetMenu(menuName = "MattrifiedGames/PlayUISFXEventSO")]
    public class PlayUISFXEventSO : IntEventSO
    {
        /* 0 Cancel
         * 1 Char Select Kiss
         * 2 Confirm 1
         * 3 Confirm 2 (Splash)
         * 4 Move
         * 5 Pause
         * 6 Reject
         * 7 Volume Adj
         * 8 Volume VO Adj
         * 9 Kiss
         * 10 Add Char
         * 11 Cancel Char
         * 12 Lock
         * 13 Mode Select
         * 14 Players Ready
         */

        public void PlayCancel() { Raise(0); }

        public void PlayCharSelectKiss()
        {
            Raise(1);
        }

        public void PlayConfirm1()
        {
            Raise(2);
        }

        public void PlayConfirm2()
        {
            Raise(3);
        }

        public void PlayMove()
        {
            Raise(4);

        }

        public void PlayPause()
        {
            Raise(5);
        }

        public void PlayReject()
        {
            Raise(6);
        }

        public void PlayVolumeAdjust()
        {
            Raise(7);
        }

        public void PlayVOVolumeAdjust()
        {
            Raise(8);
        }

        public void PlayKiss()
        {
            Raise(9);
        }

        public void PlayAddCharacter()
        {
            Raise(10);
        }

        public void PlayRemoveCharacter()
        {
            Raise(11);
        }

        public void PlayLock()
        {
            Raise(12);
        }

        public void PlayModeSelect()
        {
            Raise(13);
        }

        public void PlayReady()
        {
            Raise(14);
        }
    }
}