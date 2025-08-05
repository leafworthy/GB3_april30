using System.Collections.Generic;
using UnityEngine;
using FunkyCode.Utilities;

namespace FunkyCode.LightShape
{
	public class SpriteShape : Base
	{
		private Sprite originalSprite;
		private SpriteRenderer spriteRenderer;

		private VirtualSpriteRenderer virtualSpriteRenderer = new VirtualSpriteRenderer();

		public override int GetSortingLayer()
		{
			SpriteRenderer sr = GetSpriteRenderer();
			if (sr == null)
			{
				return 0;
			}
			return(UnityEngine.SortingLayer.GetLayerValueFromID(sr.sortingLayerID));
		}

        public override int GetSortingOrder()
        {
			SpriteRenderer spriteRenderer = GetSpriteRenderer();

			if (spriteRenderer != null)
			{
				return(spriteRenderer.sortingOrder);
			}

            return(0);
        }

		public override List<Polygon2> GetPolygonsLocal()
		{
			if (LocalPolygons == null)
			{
				LocalPolygons = new List<Polygon2>();

				SpriteRenderer sr = GetSpriteRenderer();
				if (sr == null)
				{
					Debug.LogWarning("Light Collider 2D: Cannot access sprite renderer (Sprite Shape Local)", transform.gameObject);
					return(LocalPolygons);
				}

				Vector2 v1, v2, v3, v4;

				if (sr.drawMode == SpriteDrawMode.Tiled && sr.tileMode == SpriteTileMode.Continuous)
				{
					float rot = transform.eulerAngles.z;
					Vector2 size = transform.localScale * sr.size * 0.5f;
					Vector2 pos = Vector3.zero;

					rot = rot * Mathf.Deg2Rad + Mathf.PI;

					float rectAngle = Mathf.Atan2(size.y, size.x);
					float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

					v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
					v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
					v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
					v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
				}
					else
				{
					virtualSpriteRenderer.Set(sr);

					Vector2 position = Vector3.zero;
					Vector2 scale = transform.localScale;
					float rotation = transform.eulerAngles.z;

					SpriteTransform spriteTransform = new SpriteTransform(virtualSpriteRenderer, position, scale, rotation);

					float rot = spriteTransform.rotation;
					Vector2 size = spriteTransform.scale;
					Vector2 pos = spriteTransform.position;

					rot = rot * Mathf.Deg2Rad + Mathf.PI;

					float rectAngle = Mathf.Atan2(size.y, size.x);
					float dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

					v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
					v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist, pos.y + Mathf.Sin(-rectAngle + rot) * dist);
					v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
					v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist, pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);
				}

				Polygon2 polygon = new Polygon2(4);

				polygon.points[0] = v1;
				polygon.points[1] = v2;
				polygon.points[2] = v3;
				polygon.points[3] = v4;

				LocalPolygons.Add(polygon);
			}

			return(LocalPolygons);
		}

		public override List<Polygon2> GetPolygonsWorld()
		{
			if (WorldPolygons == null)
			{
				Vector2 v1, v2, v3, v4;

				if (WorldCache == null)
				{
					WorldPolygons = new List<Polygon2>();

					SpriteRenderer sr = GetSpriteRenderer();
					if (sr == null)
					{
						Debug.LogWarning("Light Collider 2D: Cannot access sprite renderer (Sprite Shape), using transform bounds as fallback",
							transform.gameObject);

						// Fallback: create a unit square polygon based on transform
						CreateFallbackPolygon();
						return WorldPolygons;
					}

					// ... rest of existing sprite renderer logic
				}
				// ... rest of existing logic
			}

			return (WorldPolygons);
		}

		private void CreateFallbackPolygon()
		{
			// Create a simple unit square centered on the transform
			Vector2 pos = transform.position;
			Vector2 scale = transform.lossyScale * 0.5f; // Half extents
			float rot = transform.eulerAngles.z * Mathf.Deg2Rad;

			float cos = Mathf.Cos(rot);
			float sin = Mathf.Sin(rot);

			// Calculate rotated corners
			Vector2 v1 = new Vector2(pos.x + (scale.x * cos - scale.y * sin), pos.y + (scale.x * sin + scale.y * cos));
			Vector2 v2 = new Vector2(pos.x + (-scale.x * cos - scale.y * sin), pos.y + (-scale.x * sin + scale.y * cos));
			Vector2 v3 = new Vector2(pos.x + (-scale.x * cos + scale.y * sin), pos.y + (-scale.x * sin - scale.y * cos));
			Vector2 v4 = new Vector2(pos.x + (scale.x * cos + scale.y * sin), pos.y + (scale.x * sin - scale.y * cos));

			Polygon2 polygon = new Polygon2(4);
			polygon.points[0] = v1;
			polygon.points[1] = v2;
			polygon.points[2] = v3;
			polygon.points[3] = v4;

			WorldPolygons.Add(polygon);
		}

		public override void ResetLocal()
		{
			base.ResetLocal();

			originalSprite = null;
		}

		public SpriteRenderer GetSpriteRenderer()
		{
			if (spriteRenderer != null)
			{
				return(spriteRenderer);
			}

			if (transform == null)
			{
				return null; // Return null instead of spriteRenderer (which is also null)
			}

			if (spriteRenderer == null)
			{
				spriteRenderer = transform.GetComponent<SpriteRenderer>();
			}

			return(spriteRenderer);
		}

		public Sprite GetOriginalSprite()
		{
            if (originalSprite == null)
			{
                GetSpriteRenderer();

                if (spriteRenderer != null)
				{
                    originalSprite = spriteRenderer.sprite;
                }
            }
			return(originalSprite);
		}
	}
}
