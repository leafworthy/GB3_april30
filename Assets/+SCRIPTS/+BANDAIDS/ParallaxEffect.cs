using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    // Public fields for parallax configuration
    public float parallaxMultiplier = 0.5f; // Controls parallax speed
    public bool enableXAxis = true; // Toggle X-axis parallax
    public bool enableYAxis = true; // Toggle Y-axis parallax

    // Private fields for tracking
    private Vector3 previousCameraPosition; // Track camera's last position
    private Camera mainCamera; // Cache camera reference



    void OnEnable()
    {
        // Re-acquire camera reference when object is enabled (handles scene changes)
        mainCamera = __SCRIPTS.Cursor.CursorManager.GetCamera();

        // Update the camera position reference
        if (mainCamera != null)
        {
            previousCameraPosition = mainCamera.transform.position;
        }
        else
        {
            Debug.LogWarning("ParallaxEffect: Camera not available in OnEnable()");
        }
    }

    void LateUpdate()
    {
        // Check if camera is available (handle null case)
        if (mainCamera == null)
        {
            return;
        }

        // Calculate camera movement delta
        Vector3 currentCameraPosition = mainCamera.transform.position;
        Vector3 cameraDelta = currentCameraPosition - previousCameraPosition;

        // Apply parallax offset based on delta and multiplier
        Vector3 parallaxOffset = Vector3.zero;

        // Respect axis enable flags when applying movement
        if (enableXAxis)
        {
            parallaxOffset.x = cameraDelta.x * parallaxMultiplier;
        }

        if (enableYAxis)
        {
            parallaxOffset.y = cameraDelta.y * parallaxMultiplier;
        }

        // Apply the parallax offset to this object's position
        transform.position += parallaxOffset;

        // Update previous camera position
        previousCameraPosition = currentCameraPosition;
    }
}
