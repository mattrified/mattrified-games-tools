using System.Collections.Generic;
using UnityEngine;

public class RandomizeChildren : MonoBehaviour
{
    [SerializeField()]
    bool performOnAwake;

    public Vector3 minPosRange;
    public Vector3 maxPosRange;

    public Vector3 minRotRange;
    public Vector3 maxRotRange;

    public Vector3 minScaleRange;
    public Vector3 maxScaleRange;

    [SerializeField()]
    bool storedOriginals;

    [SerializeField()]
    List<Vector3> originalPos = new List<Vector3>();

    [SerializeField()]
    List<Vector3> originalRot = new List<Vector3>();

    [SerializeField()]
    List<Vector3> originalScale = new List<Vector3>();

    [Range(0f,1f)]
    public float reduceChildPercent = 0;

    [SerializeField()]
    bool unifyScale;

    public void Awake()
    {
        if (performOnAwake)
            Randomize();
    }

    [ContextMenu("Randomize")]
    private void Randomize()
    {
        if (!storedOriginals)
        {
            CacheOriginalInfo();
            storedOriginals = true;
        }

        int reduceChildCount = Mathf.RoundToInt(reduceChildPercent * originalPos.Count);
        List<int> hide = new List<int>();
        for (int i = 0, len = originalPos.Count; i < len; i++)
        {
            hide.Add(i);
        }
        for (int i = 0; i < reduceChildCount; i++)
        {
            hide.RemoveAt(Random.Range(0, hide.Count));
        }

        for (int i = 0, len = originalPos.Count; i < len; i++)
        {
            var t = transform.GetChild(i);

            t.gameObject.SetActive(hide.Contains(i));
            if (!t.gameObject.activeSelf)
                continue;

            t.localPosition = originalPos[i] + RandomizeVector(minPosRange, maxPosRange);
            t.localEulerAngles = originalRot[i];
            t.localRotation *= Quaternion.Euler(RandomizeVector(minRotRange, maxRotRange));

            if (!unifyScale)
                t.localScale = originalScale[i] + RandomizeVector(minScaleRange, maxScaleRange);
            else
                t.localScale = originalScale[i] + Vector3.one * Random.Range(minScaleRange.x, maxScaleRange.x);
        }
    }

    [ContextMenu("Restore")]
    private void Restore()
    {
        if (!storedOriginals)
            return;

        for (int i = 0, len = originalPos.Count; i < len; i++)
        {
            var t = transform.GetChild(i);
            t.localPosition = originalPos[i];
            t.localEulerAngles = originalRot[i];
            t.localScale = originalScale[i];
        }
    }

    Vector3 RandomizeVector(Vector3 a, Vector3 b)
    {
        return new Vector3(Mathf.Lerp(a.x, b.x, UnityEngine.Random.value),
            Mathf.Lerp(a.y, b.y, UnityEngine.Random.value),
            Mathf.Lerp(a.z, b.z, UnityEngine.Random.value));
    }

    private void CacheOriginalInfo()
    {
        originalPos = new List<Vector3>();
        originalScale = new List<Vector3>();
        originalRot = new List<Vector3>();
        
        foreach (Transform child in transform)
        {
            originalPos.Add(child.localPosition);
            originalScale.Add(child.localScale);
            originalRot.Add(child.localEulerAngles);
        }
    }
}