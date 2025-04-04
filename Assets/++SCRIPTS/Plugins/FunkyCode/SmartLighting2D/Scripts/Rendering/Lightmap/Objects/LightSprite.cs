﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Universal.Objects;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities;
using UnityEngine;
using Sprite = UnityEngine.Sprite;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Lightmap.Objects
{	
    public static class LightSprite
	{
		public static class Script
		{	
			static public void DrawScriptable(Scriptable.LightSprite2D id, UnityEngine.Camera camera)
			{
				if (id.Sprite == null)
				{
					return;
				}

				if (!id.InCamera(camera))
				{
					return;
				}

				Vector2 position = id.Position;
				position.x -= camera.transform.position.x;
				position.y -= camera.transform.position.y;

				Vector2 scale = id.Scale;
				float rot = id.Rotation;

				UnityEngine.Material material = Lighting2D.materials.GetAdditive(); // get light sprite material?
				material.mainTexture = id.Sprite.texture;

				VirtualSpriteRenderer virtualSprite = new VirtualSpriteRenderer();
				virtualSprite.sprite = id.Sprite;

				GLExtended.color = new Color(id.Color.r * 0.5f, id.Color.g * 0.5f, id.Color.b * 0.5f, id.Color.a);

				material.SetPass(0);

				GL.Begin (GL.QUADS);

				Universal.Objects.Sprite.Pass.Draw(id.spriteMeshObject, virtualSprite, position, scale, rot);

				GL.End ();
				
				material.mainTexture = null;
			}
		}

		static public class Pass
		{
			public static Texture2D currentTexture;

			static public void Draw(LightSprite2D id, UnityEngine.Camera camera)
			{
				if (id.GetSprite() == null)
				{
					return;
				}

				if (!id.InCamera(camera))
				{
					return;
				}

				UnityEngine.Material material = Lighting2D.materials.GetLightSprite();

				if (material == null)
				{
					return;
				}

				Sprite sprite = id.GetSprite();

				if (sprite == null)
				{
					return;
				}

				Texture2D texture = sprite.texture;

				if (texture == null)
				{
					return;
				}

				if (texture != currentTexture)
				{
					if (currentTexture != null)
					{
						GL.End();
					}

					currentTexture = texture;
                    material.mainTexture = currentTexture;

                    material.SetPass(0);
                    GL.Begin(GL.QUADS);
				}
				
				Vector2 position = LightingPosition.GetPosition2D(id.transform.position);
				position -= LightingPosition.GetPosition2D(camera.transform.position);

				Vector2 scale = LightingPosition.GetPosition2D(id.transform.lossyScale);
				scale.x *= id.lightSpriteTransform.scale.x;
				scale.y *= id.lightSpriteTransform.scale.y;

				float rot = id.lightSpriteTransform.rotation;
				
				if (id.lightSpriteTransform.applyRotation)
				{
					rot += id.transform.rotation.eulerAngles.z;
				}
	
				float ratio = (float)texture.width / (float)texture.height;
				float type = id.type == LightSprite2D.Type.Mask ? 1 : 0;
				float glow = id.glowMode.enable ? id.glowMode.glowRadius : 0;

				GLExtended.color = id.color;

				GL.MultiTexCoord3(1, glow, ratio, type);
	
				Universal.Objects.Sprite.MultiPass.Draw(id.spriteMeshObject, id.spriteRenderer, position + id.lightSpriteTransform.position, scale, rot);
			}
		}

		static public class Simple
		{	
			static public void Draw(LightSprite2D id, UnityEngine.Camera camera)
			{
				if (id.GetSprite() == null)
				{
					return;
				}

				if (!id.InCamera(camera))
				{
					return;
				}

				UnityEngine.Material material = Lighting2D.materials.GetLightSprite();

				if (material == null)
				{
					return;
				}

				Vector2 position = LightingPosition.GetPosition2D(id.transform.position);
				position -= LightingPosition.GetPosition2D(camera.transform.position);

				Vector2 scale = LightingPosition.GetPosition2D(id.transform.lossyScale);
				scale.x *= id.lightSpriteTransform.scale.x;
				scale.y *= id.lightSpriteTransform.scale.y;

				float rot = id.lightSpriteTransform.rotation;

				if (id.lightSpriteTransform.applyRotation)
				{
					rot += id.transform.rotation.eulerAngles.z;
				}

				Sprite sprite = id.GetSprite();

				if (sprite == null)
				{
					return;
				}

				Texture2D texture = sprite.texture;

				if (texture == null)
				{
					return;
				}
	
				float ratio = (float)texture.width / (float)texture.height;
				float type = id.type == LightSprite2D.Type.Mask ? 1 : 0;
				float glow = id.glowMode.enable ? id.glowMode.glowRadius : 0;

				material.mainTexture = texture;
				material.SetPass(0);

				GL.Begin (GL.QUADS);

				GLExtended.color = id.color;

				GL.MultiTexCoord3(1, glow, ratio, type);
	
				Universal.Objects.Sprite.MultiPass.Draw(id.spriteMeshObject, id.spriteRenderer, position + id.lightSpriteTransform.position, scale, rot);

				GL.End ();
				
				material.mainTexture = null;
			}
		}
	}
}
