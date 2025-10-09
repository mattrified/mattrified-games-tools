using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class FrameBasedTimer : MonoBehaviour
{
    public int minFrame, maxFrame;
    int frame;

    public UnityEvent onCompleteEvent;

    public bool beginOnAwake = true;

    private void Awake()
    {
        if (beginOnAwake)
            ResetTimer();
        else
            enabled = false;
    }

    private void FixedUpdate()
    {
        if (--frame <= 0)
        {
            enabled = false;
            onCompleteEvent.Invoke();
        }
    }

    public void ResetTimer()
    {
        frame = Random.Range(minFrame, maxFrame);
        enabled = true;
    }

    public void SetTimer(int frames)
    {
        frame = frames;
        enabled = true;
    }
}
