using FunkyCode.SmartLighting2D.Scripts.Misc;
using UnityEngine;

namespace FunkyCode.SmartLighting2D.Scripts.Components.LightCollider2D
{
	public class LightColliderTransform
	{
		private bool update = true;

		public bool UpdateNeeded
		{
			get => update;
			set => update = value;
		}

		public Vector2 position = Vector2.zero;
		public Vector2 scale = Vector3.zero;
		public float rotation = 0;
		public float shadowHeight = 0;
		public float shadowTranslucency = 0;

		private Vector3 position3D = Vector3.zero;
		private bool flipX = false;
		private bool flipY = false;
		private Vector2 size = Vector2.one;

		private LightColliderShape shape;

		private FunkyCode.LightCollider2D lightCollider;

		public void SetShape(LightColliderShape shape, FunkyCode.LightCollider2D lightCollider)
		{
			this.shape = shape;

			this.lightCollider = lightCollider;
		}

		public void  Reset()
		{
			position = Vector2.zero;
			rotation = 0;
			scale = Vector3.zero;
		}

		private void UpdateTransform(Transform transform)
		{
			Vector3 newPosition3D = transform.position;
			Vector2 position2D = LightingPosition.GetPosition2D(transform.position);

			Vector2 scale2D = transform.lossyScale;
			float rotation2D = LightingPosition.GetRotation2D(transform);

			if (scale != scale2D)
			{
				scale = scale2D;

				update = true;
			}

			if (rotation != rotation2D)
			{
				rotation = rotation2D;

				update = true;
			}

			if (position3D != newPosition3D)
			{
				position3D = newPosition3D;

				update = true;
			}

			if (position != position2D)
			{
				position = position2D;

				update = true;
			}
		}

		public void Update(bool force)
		{
			Transform transform = shape.transform;

			if (transform == null)
			{
				return;
			}

			if (transform.hasChanged || force)
			{
				transform.hasChanged = false;

				UpdateTransform(transform);
			}

			if (shadowTranslucency != lightCollider.shadowTranslucency)
			{
				shadowTranslucency = lightCollider.shadowTranslucency;

				update = true;
			}

			bool checkShapeSprite = shape.maskType == FunkyCode.LightCollider2D.MaskType.SpritePhysicsShape || shape.shadowType == FunkyCode.LightCollider2D.ShadowType.SpritePhysicsShape;
			bool checkMaskSprite = shape.maskType == FunkyCode.LightCollider2D.MaskType.Sprite || shape.maskType == FunkyCode.LightCollider2D.MaskType.BumpedSprite;

			if (checkShapeSprite || checkMaskSprite)
			{
				SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

				if (spriteRenderer != null)
				{
					if (spriteRenderer.size != size)
					{
						size = spriteRenderer.size;

						update = true;
					}

					if (spriteRenderer.flipX != flipX || spriteRenderer.flipY != flipY)
					{
						flipX = spriteRenderer.flipX;
						flipY = spriteRenderer.flipY;

						shape.ResetWorld();

						update = true;
					}

					if (shape.spriteShape.GetOriginalSprite() != spriteRenderer.sprite)
					{
						shape.ResetLocal();

						update = true;
					}
				}
			}

			bool checkShapeMesh = shape.maskType == FunkyCode.LightCollider2D.MaskType.MeshRenderer || shape.shadowType == FunkyCode.LightCollider2D.ShadowType.MeshRenderer;

			if (checkShapeMesh)
			{
				MeshFilter meshFilter = shape.meshShape.GetMeshFilter();

				if (meshFilter != null)
				{
					if (meshFilter.sharedMesh != shape.meshShape.mesh)
					{
						shape.ResetLocal();

						update = true;
					}
				}
			}
		}
	}
}
