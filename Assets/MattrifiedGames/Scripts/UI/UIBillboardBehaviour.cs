using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBillboardBehaviour : MonoBehaviour
{
    Camera cam;
    Transform cameraTransform;
    bool ready = false;

    Transform myTransform;

    private IEnumerator Start()
    {
        myTransform = transform;

        var eof = new WaitForEndOfFrame();
        while ((object)cam == null)
        {
            cam = FindObjectOfType<Camera>();
            yield return eof;
        }
        cameraTransform = cam.transform;
        ready = true;
    }

    private void LateUpdate()
    {
        if (!ready)
            return;

        myTransform.rotation = cameraTransform.rotation;
    }

}
