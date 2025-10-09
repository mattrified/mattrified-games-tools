using UnityEngine;
using System.Collections;

/// <summary>
/// World to UI Behaviour is a UI behaviour that displays info in UI based on world position.
/// The idea with this abstract class takes in world to UI info.
/// This info is then used to setup howt he UI is displayed.
/// So different prompts would be created or setup per game.
/// </summary>
public abstract class WorldToUIBehaviour<T> : MonoBehaviour where T : WorldToUIInfo
{
    /// <summary>
    /// The camera used to determine the viewport space.
    /// </summary>
    public Camera cam;

    /// <summary>
    /// The information displayed
    /// </summary>
    [SerializeField()]
    protected T displayedInfo;

    /// <summary>
    /// The viewpoint position the ui element is displayed.
    /// </summary>
    protected Vector3 viewpoint;

    private void Awake()
    {
        AssignDisplayInfo(displayedInfo);
    }

    public void AssignDisplayInfo(T worldInfo)
    {
        displayedInfo = worldInfo;
        Setup();
    }

    public virtual void Setup()
    {
        enabled = (displayedInfo != null && displayedInfo.trackedTransform != null);
    }

    public void UnassignDisplayInfo(T worldInfo)
    {
        if (displayedInfo == worldInfo)
        {
            displayedInfo = null;
            Setup();
        }
    }

    public void LateUpdate()
    {
        // TODO:  Make sure viewpoint sizes are different.
        viewpoint = 2f * cam.WorldToViewportPoint(displayedInfo.trackedTransform.position);
        viewpoint.x -= 1f;
        viewpoint.y -= 1f;

        viewpoint.x *= 640;
        viewpoint.y *= 360f;

        transform.localPosition = viewpoint;
    }
}