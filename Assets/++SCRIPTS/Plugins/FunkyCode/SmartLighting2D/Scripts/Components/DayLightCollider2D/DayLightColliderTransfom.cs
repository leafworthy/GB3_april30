﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Day;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.DayLightCollider2D
{
	public class DayLightingColliderTransform
	{
		public bool updateNeeded = false;

		public Vector2 position = Vector2.zero;
		public Vector2 scale = Vector3.zero;
		public float rotation = 0;

		private bool flipX = false;
		private bool flipY = false;

		private float height = 0;

		private float sunDirection = 0;
		private float sunHeight = 1;

		private DayLightColliderShape shape;

		public void Reset() {
			position = Vector2.zero;
			rotation = 0;
			scale = Vector3.zero;
		}

		public void SetShape(DayLightColliderShape shape) {
			this.shape = shape;
		}

		public void Update() {
			if (shape == null) {
				return;
			}

			if (shape.transform == null) {
				return;
			}

			Transform transform = shape.transform;

			Vector2 scale2D = transform.lossyScale;
			Vector2 position2D = transform.position;
			float rotation2D = transform.rotation.eulerAngles.z;

			SpriteRenderer spriteRenderer = shape.spriteShape.GetSpriteRenderer();

			updateNeeded = false;

			if (position != position2D) {

				position = position2D;

				// does not update shadow
			}

			if (sunDirection != Lighting2D.DayLightingSettings.direction) {
				sunDirection = Lighting2D.DayLightingSettings.direction;

				updateNeeded = true;
			}

			if (sunHeight != Lighting2D.DayLightingSettings.height) {
				sunHeight = Lighting2D.DayLightingSettings.height;

				updateNeeded = true;
			}

			if (scale != scale2D) {
				scale = scale2D;

				updateNeeded = true;
			}

			if (rotation != rotation2D) {
				rotation = rotation2D;

				updateNeeded = true;
			}

			if (height != shape.height) {
				height = shape.height;

				updateNeeded = true;
			}

			// Unnecesary check
			if (shape.height < 0.01f) {
				shape.height = 0.01f;
			}

			if (shape.shadowType == SmartLighting2D.Components.Day.DayLightCollider2D.ShadowType.SpritePhysicsShape) {
				if (spriteRenderer != null) {
					if (spriteRenderer.flipX != flipX || spriteRenderer.flipY != flipY) {
						flipX = spriteRenderer.flipX;
						flipY = spriteRenderer.flipY;

						updateNeeded = true;

						shape.ResetLocal(); // World
					}

					/* Sprite frame change
					if (shape.GetOriginalSprite() != spriteRenderer.sprite) {
						shape.SetOriginalSprite(spriteRenderer.sprite);
						shape.SetAtlasSprite(null); // Only For Sprite Mask?

						moved = true;

						shape.Reset(); // Local
					}
					*/
				}
			}

			/* Sprite Frame Change
			if (shape.maskType == LightingCollider2D.MaskType.Sprite) {
				if (spriteRenderer != null && shape.GetOriginalSprite() != spriteRenderer.sprite) {
					shape.SetOriginalSprite(spriteRenderer.sprite);
					shape.SetAtlasSprite(null);

					moved = true;
				}
			}
			/*/
		}
	}




	public class DayLightTilemapColliderTransform {
		public bool moved = false;

		//public Vector2 position = Vector2.zero;
		//public Vector2 scale = Vector3.zero;
		//public float rotation = 0;

		private float height = 0;

		private float sunDirection = 0;
		private float sunHeight = 1;

		public void Update(DayLightTilemapCollider2D id) {
			Transform transform = id.transform;

			moved = false;

			if (sunDirection != Lighting2D.DayLightingSettings.direction) {
				sunDirection = Lighting2D.DayLightingSettings.direction;

				moved = true;
			}

			if (sunHeight != Lighting2D.DayLightingSettings.height) {
				sunHeight = Lighting2D.DayLightingSettings.height;

				moved = true;
			}

			if (height != id.height) {
				height = id.height;

				moved = true;
			}

			// Unnecesary check
			if (height < 0.01f) {
				height = 0.01f;
			}
		}
	}
}
