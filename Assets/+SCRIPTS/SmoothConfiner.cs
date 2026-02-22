using Unity.Cinemachine;
using UnityEngine;

public class SmoothConfiner : CinemachineExtension
{
	public Bounds worldBounds;
	public float softZoneWidth = 1.5f;
	[Range(0, 1)] public float softness = 0.85f;

	protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
	{
		// Run AFTER everything including Aim, at the very end
		if (stage != CinemachineCore.Stage.Finalize) return;

		Vector3 pos = state.RawPosition;
		pos.x = SoftClamp(pos.x, worldBounds.min.x, worldBounds.max.x, softZoneWidth, softness);
		pos.y = SoftClamp(pos.y, worldBounds.min.y, worldBounds.max.y, softZoneWidth, softness);
		state.RawPosition = pos;

		// Re-aim at the correct target from the new clamped position
		Transform lookAtTarget = vcam.LookAt != null ? vcam.LookAt : vcam.Follow;
		if (lookAtTarget != null)
		{
			Vector3 dir = lookAtTarget.position - pos;
			if (dir.sqrMagnitude > 0.0001f)
				state.RawOrientation = Quaternion.LookRotation(dir, Vector3.up);
		}
	}

	float SoftClamp(float value, float min, float max, float zone, float softness)
	{
		if (value < min + zone)
		{
			float t = 1f - ((value - min) / zone);
			return value + t * t * zone * (1f - softness);
		}

		if (value > max - zone)
		{
			float t = 1f - ((max - value) / zone);
			return value - t * t * zone * (1f - softness);
		}

		return value;
	}
}
