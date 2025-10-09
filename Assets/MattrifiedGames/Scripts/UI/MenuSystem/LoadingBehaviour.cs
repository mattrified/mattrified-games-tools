using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingBehaviour : MonoBehaviour
{
    [SerializeField()]
    CanvasGroup canvasGroup;

    [SerializeField(), Range(0, 1)]
    float alpha;

    float alphaTarget = 0;

    public float rate;

    // Loading screens are never active on Load.
    private void Awake()
    {
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        alpha = Mathf.MoveTowards(alpha, alphaTarget, Time.deltaTime * rate);
        canvasGroup.alpha = alpha;
    }

    public void BeginFadeIn()
    {
        alphaTarget = 1f;
    }

    public void BeginFadeOut()
    {
        alphaTarget = 0f;
    }

    public bool FadeInComplete
    {
        get
        {
            return alpha >= 1f;
        }
    }

    public bool FadeOutComplete
    {
        get
        {
            return alpha <= 0f;
        }
    }

    public void BeginFadeRoutine(float waitTime, System.Func<IEnumerator> coroutineFunction, System.Action endAction)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(waitTime, coroutineFunction, endAction));
    }

    IEnumerator FadeRoutine(float waitTime, System.Func<IEnumerator> coroutineFunction, System.Action endAction)
    {
        BeginFadeIn();

        while (!FadeInComplete)
            yield return null;

        if (coroutineFunction != null)
        {
            var routine = StartCoroutine(coroutineFunction());

            while (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                yield return null;
            }

            yield return routine;
        }
        else
        {
            yield return new WaitForSeconds(waitTime);
        }

        BeginFadeOut();

        while (!FadeOutComplete)
            yield return null;

        gameObject.SetActive(false);
    }
}