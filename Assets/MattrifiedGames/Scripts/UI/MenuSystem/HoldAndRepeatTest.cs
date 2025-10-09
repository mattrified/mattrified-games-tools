using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable()]
public struct HoldAndRepeatTest
{
    byte holdState;

    /// <summary>
    /// The amount of time this action has been held.
    /// </summary>
    float startTime;

    /// <summary>
    /// After how many seconds will the next repeated input occur.
    /// </summary>
    public float initialDelay;

    /// <summary>
    /// The amount of seconds before the repeat test will return true again.
    /// </summary>
    public float activeDelay;

    public bool Test(bool isDown)
    {
        switch (holdState)
        {
            case 0:
                if (isDown)
                {
                    holdState = 1;
                    startTime = Time.timeSinceLevelLoad;
                    return true;
                }
                return false;
            case 1:
                if (isDown)
                {
                    if (Time.timeSinceLevelLoad >= startTime + initialDelay)
                    {
                        startTime = Time.timeSinceLevelLoad;
                        holdState = 2;
                        return true;
                    }
                    return false;
                }
                holdState = 0;
                return false;
            case 2:
                if (!isDown)
                {
                    holdState = 0;
                    return false;
                }

                if (Time.timeSinceLevelLoad >= startTime + activeDelay)
                {
                    startTime = Time.timeSinceLevelLoad;
                    return true;
                }
                return false;
            default:
                Debug.LogWarning("Hold and repeat tests should never have a hold state other than 0, 1, or 2.");
                return false;
        }
    }
}
