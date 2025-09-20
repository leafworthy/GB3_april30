using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
	// Public fields for parallax configuration
	public float parallaxMultiplier = 0.5f; // Controls parallax speed
	public bool enableXAxis = true; // Toggle X-axis parallax
	public bool enableYAxis = true; // Toggle Y-axis parallax

	// Private fields for tracking
	private Vector3 worldOrigin; // Fixed reference point for camera
	private Vector3 objectOrigin; // Original position of this object
	private Camera mainCamera; // Cache camera reference

	private void OnEnable()
	{
		// Re-acquire camera reference when object is enabled (handles scene changes)
		mainCamera = __SCRIPTS.Cursor.CursorManager.GetCamera();

		// Store original positions as reference points
		if (mainCamera != null)
		{
			worldOrigin = mainCamera.transform.position;
			objectOrigin = transform.position;
		}
	}

	private void LateUpdate()
	{
		// Check if camera is available (handle null case)
		if (mainCamera == null) return;

		// Calculate total camera offset from world origin
		var totalCameraOffset = mainCamera.transform.position - worldOrigin;

		// Calculate parallax position based on total offset
		var parallaxOffset = Vector3.zero;

		// Respect axis enable flags when applying movement
		if (enableXAxis) parallaxOffset.x = totalCameraOffset.x * parallaxMultiplier;

		if (enableYAxis) parallaxOffset.y = totalCameraOffset.y * parallaxMultiplier;

		// Set position based on object origin plus parallax offset
		transform.position = objectOrigin + parallaxOffset;
	}
}
