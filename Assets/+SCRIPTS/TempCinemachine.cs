using System.Collections;
using __SCRIPTS;
using Unity.Cinemachine;
using UnityEngine;

public class TempCinemachine : Singleton<TempCinemachine>
{
    /// <summary>
    /// Creates a temporary Cinemachine camera that follows a target for a limited duration.
    /// It becomes the highest-priority camera, optionally sets zoom (FOV or OrthoSize),
    /// and blends in/out smoothly.
    /// </summary>
    public static CinemachineCamera CreateFollowCameraTemporary(
        Transform target,
        float duration,
        float? zoom = null,               // optional zoom override (FOV or OrthoSize)
        bool orthographic = false,        // whether to use orthographic projection
        float priorityOffset = 100f,
        bool destroyAfter = true,
        string name = "TempVcam")
    {
        if (target == null)
        {
            Debug.LogError("[TempCinemachine] target is null.");
            return null;
        }

        var vcam = CreateVcam(target, name, zoom, orthographic);

        // Find current highest priority
        int highest = 0;
        var allCams = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);
        for (int i = 0; i < allCams.Length; i++)
        {
            var c = allCams[i];
            if (c != null && c != vcam && c.Priority > highest)
                highest = c.Priority;
        }

        vcam.Priority = highest + Mathf.RoundToInt(Mathf.Abs(priorityOffset));

        if (duration > 0f)
            I.StartCoroutine(I.TemporaryPriorityCoroutine(vcam, duration, destroyAfter, highest));

        return vcam;
    }

    private static CinemachineCamera CreateVcam(Transform target, string name, float? zoom, bool orthographic)
    {
        var vcamGO = new GameObject(name);
        var vcam = vcamGO.AddComponent<CinemachineCamera>();

        vcamGO.transform.position = target.position + Vector3.back * 10f;
        vcam.Follow = target;
        vcam.LookAt = target;

        // Get lens reference
        var lens = vcam.Lens;

        // Use ModeOverride to set projection type
        lens.ModeOverride = orthographic
            ? LensSettings.OverrideModes.Orthographic
            : LensSettings.OverrideModes.None;

        // Apply zoom
        if (orthographic)
        {
            if (zoom.HasValue)
                lens.OrthographicSize = zoom.Value;
            else if (Camera.main != null)
                lens.OrthographicSize = Camera.main.orthographicSize;
            else
                lens.OrthographicSize = 5f;
        }
        else
        {
            if (zoom.HasValue)
                lens.FieldOfView = zoom.Value;
            else if (Camera.main != null)
                lens.FieldOfView = Camera.main.fieldOfView;
            else
                lens.FieldOfView = 60f;
        }

        vcam.Lens = lens;
        return vcam;
    }

    private IEnumerator TemporaryPriorityCoroutine(
        CinemachineCamera vcam, float duration, bool destroyAfter, int previousHighestPriority)
    {
        if (vcam == null)
            yield break;

        float end = Time.unscaledTime + duration;
        while (Time.unscaledTime < end)
        {
            if (vcam == null) yield break;
            yield return null;
        }

        if (vcam == null) yield break;

        // Smooth out: lower priority, then destroy after blend time
        vcam.Priority = Mathf.Max(0, previousHighestPriority - 1);

        var brain = Camera.main?.GetComponent<CinemachineBrain>();
        float blendTime = brain != null ? brain.DefaultBlend.BlendTime : 0.5f;
        yield return new WaitForSecondsRealtime(5);

        if (destroyAfter && vcam != null)
            Destroy(vcam.gameObject);
    }
}
