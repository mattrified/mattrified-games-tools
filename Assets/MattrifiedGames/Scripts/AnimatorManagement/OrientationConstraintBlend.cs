using MattrifiedGames.ManagedAnimation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationConstraintBlend : MonoBehaviour
{
    public Transform[] mainItems;
    public Transform[] driversA;
    public Transform[] driversB;

    [SerializeField()]
    bool useParameter;

    public StringHash parameterName;

    public StringHash mirrorParameter;

    [Range(0f, 1f)]
    public float value = 0f;

    [Range(0f, 1f)]
    public float goalValue = 0f;

    public bool mirror;

    Animator anim;

    [SerializeField()]
    bool rotationOnly;

    public float moveTowardsRate = 30f;

    [Range(0f, 1f)]
    public float mirrorValue;

    public float Value
    {
        get
        {
            return value;
        }

        set
        {
            this.value = value;
        }
    }

    [ContextMenu("Drive Items")]
    void PopulateItems()
    {
        List<Transform> miList = new List<Transform>();
        List<Transform> daList = new List<Transform>();
        List<Transform> dbList = new List<Transform>();

        RecursiveChildFind(mainItems[0], miList);
        RecursiveChildFind(driversA[0], daList);
        RecursiveChildFind(driversB[0], dbList);

        mainItems = miList.ToArray();
        driversA = daList.ToArray();
        driversB = dbList.ToArray();
    }

    private void RecursiveChildFind(Transform start, List<Transform> miList)
    {
        miList.Add(start);
        for (int i = 0; i < start.childCount; i++)
        {
            RecursiveChildFind(start.GetChild(i), miList);
        }


    }

    private void Awake()
    {
        if (mainItems.Length != driversA.Length || mainItems.Length != driversB.Length)
        {
            Debug.LogError("All three lists must be of the same length");
            enabled = false;
            return;
        }

        if (!rotationOnly)
        {
            for (int i = 1; i < mainItems.Length; i++)
            {
                mainItems[i].SetParent(mainItems[0].parent, true);
            }
        }

        if (useParameter)
        {
            anim = GetComponentInParent<Animator>();
            if (anim == null)
            {
                Debug.LogError("Cannot use parameter if there is no animator.");
                useParameter = false;
            }
        }
    }

    private void LateUpdate()
    {
        if (useParameter)
        {
            goalValue = anim.GetFloat(parameterName.Hash);
            mirror = anim.GetBool(mirrorParameter.Hash);
        }

        value = Mathf.MoveTowards(value, goalValue, moveTowardsRate * Time.deltaTime);

        if (!rotationOnly)
        {
            for (int i = 0, len = mainItems.Length; i < len; i++)
            {
                Quaternion q = Quaternion.Lerp(driversA[i].rotation, driversB[i].rotation, value);
                Vector3 p = Vector3.Lerp(driversA[i].position, driversB[i].position, value);
                mainItems[i].rotation = q;
                mainItems[i].position = p;

                if (mirror)
                {
                    Vector3 localPos = mainItems[i].localPosition;
                    localPos.x *= -1f;
                    mainItems[i].localPosition = Vector3.Lerp(mainItems[i].localPosition, localPos, mirrorValue);

                    q = mainItems[i].localRotation;
                    Quaternion mq = q;
                    mq.x *= -1;
                    mq.w *= -1;
                    mainItems[i].localRotation = Quaternion.Lerp(q, mq, mirrorValue);
                }
            }
        }
        else
        {
            for (int i = 0, len = mainItems.Length; i < len; i++)
            {
                if (!mirror)
                {
                    Quaternion q = Quaternion.Lerp(driversA[i].localRotation, driversB[i].localRotation, value);
                    mainItems[i].localRotation = q;
                }
                else
                {
                    Quaternion q = Quaternion.Lerp(driversA[i].rotation, driversB[i].rotation, value);
                    Quaternion mq = q;
                    mq.x *= -1f;
                    mq.w *= -1f;
                    mainItems[i].rotation = Quaternion.Lerp(q, mq, mirrorValue);
                }
            }
        }
    }
}
