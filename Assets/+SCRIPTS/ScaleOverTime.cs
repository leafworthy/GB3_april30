using System.Collections;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour
{
    [Header("Target Scale Range")]
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 1f);
    public Vector3 maxScale = new Vector3(2f, 2f, 1f);

    [Header("Duration Range")]
    public float minDuration = 0.5f;
    public float maxDuration = 2f;

    [Header("Options")]
    public bool playOnStart = true;
    public bool uniformScale = true; // Locks X and Y to the same random value
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Coroutine _scaleCoroutine;


    void OnEnable()
    {
	    if (playOnStart)
		    ScaleToRandom();
    }

    void OnDisable()
    {
	    transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Picks a random target scale and duration, then scales to it.
    /// </summary>
    public void ScaleToRandom()
    {
        Vector3 target;

        if (uniformScale)
        {
            float uniformValue = Random.Range(minScale.x, maxScale.x);
            target = new Vector3(uniformValue, uniformValue, 1f);
        }
        else
        {
            target = new Vector3(
                Random.Range(minScale.x, maxScale.x),
                Random.Range(minScale.y, maxScale.y),
                1f // Keep Z at 1 for sprites
            );
        }

        float duration = Random.Range(minDuration, maxDuration);
        ScaleTo(target, duration);
    }

    /// <summary>
    /// Scales the GameObject to a specific target scale over a given duration.
    /// </summary>
    public void ScaleTo(Vector3 target, float time)
    {
        if (_scaleCoroutine != null)
            StopCoroutine(_scaleCoroutine);

        _scaleCoroutine = StartCoroutine(ScaleRoutine(target, time));
    }

    private IEnumerator ScaleRoutine(Vector3 target, float time)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            float curvedT = scaleCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(startScale, target, curvedT);
            yield return null;
        }

        transform.localScale = target;
        _scaleCoroutine = null;
    }
}
