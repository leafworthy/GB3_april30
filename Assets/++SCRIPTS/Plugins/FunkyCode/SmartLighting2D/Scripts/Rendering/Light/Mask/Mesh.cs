﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Universal.Objects;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;
using Texture = UnityEngine.Texture;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Light.Mask
{
    public class Mesh
	{
        public static void Mask(Light2D light, LightCollider2D id, UnityEngine.Material material, LayerSetting layerSetting)
		{
			if (!id.InLight(light))
			{
				return;
			}

			MeshRenderer meshRenderer = id.mainShape.meshShape.GetMeshRenderer();

			if (meshRenderer == null) {
				return;
			}

			List<MeshObject> meshObjects = id.mainShape.GetMeshes();

			if (meshObjects == null) {
				return;
			}

			if (meshRenderer.sharedMaterial != null) {
				material.mainTexture = meshRenderer.sharedMaterial.mainTexture;
			} else {
				material.mainTexture = null;
			}

			Vector2 position = id.mainShape.transform2D.position - light.transform2D.position;

			Vector2 pivotPosition = id.mainShape.GetPivotPoint() - light.transform2D.position;
			GLExtended.color = LayerSettingColor.Get(pivotPosition, layerSetting, id.maskLit, 1, id.maskLitCustom);

			material.SetPass(0);

			GLExtended.DrawMesh(meshObjects, position, id.mainShape.transform2D.scale, id.mainShape.transform2D.rotation);

			material.mainTexture = null;
		}

		public static void MaskNormalMap(Light2D light, LightCollider2D id, UnityEngine.Material material, LayerSetting layerSetting) {
			if (id.InLight(light) == false) {
				return;
			}

			Texture normalTexture = id.bumpMapMode.GetBumpTexture();

            if (normalTexture == null) {
                return;
            }

			float rotation;

            material.SetTexture("_Bump", normalTexture);

			MeshRenderer meshRenderer = id.mainShape.meshShape.GetMeshRenderer();

			if (meshRenderer == null) {
				return;
			}

			List<MeshObject> meshObjects = id.mainShape.GetMeshes();

			if (meshObjects == null) {
				return;
			}

			if (meshRenderer.sharedMaterial != null) {
				material.mainTexture = meshRenderer.sharedMaterial.mainTexture;
			} else {
				material.mainTexture = null;
			}

			Vector2 position = id.mainShape.transform2D.position - light.transform2D.position;

			Vector2 pivotPosition = id.mainShape.GetPivotPoint() - light.transform2D.position;
			material.color = LayerSettingColor.Get(pivotPosition, layerSetting, id.maskLit, 1, id.maskLitCustom);

			float color = material.color.r;

			switch(id.bumpMapMode.type) {
				case NormalMapType.ObjectToLight:
					rotation = Mathf.Atan2(light.transform2D.position.y - id.mainShape.transform2D.position.y, light.transform2D.position.x - id.mainShape.transform2D.position.x);
					rotation -= Mathf.Deg2Rad * (id.mainShape.transform2D.rotation);

					material.SetFloat("_LightRX", Mathf.Cos(rotation) * 2);
					material.SetFloat("_LightRY", Mathf.Sin(rotation) * 2);
					material.SetFloat("_LightColor",  color);

				break;

				case NormalMapType.PixelToLight:
					material.SetFloat("_LightColor",  color);

					rotation = id.mainShape.transform2D.rotation * Mathf.Deg2Rad;

					Vector2 sc = id.mainShape.transform2D.scale;
					sc = sc.normalized;

					material.SetFloat("_LightX", Mathf.Cos(rotation) * sc.x );
					material.SetFloat("_LightY", Mathf.Cos(rotation) * sc.y );

					material.SetFloat("_Depth", id.bumpMapMode.depth);

					if (id.bumpMapMode.invertX) {
						material.SetFloat("_InvertX", -1);
					} else {
						material.SetFloat("_InvertX", 1);
					}

					if (id.bumpMapMode.invertY) {
						material.SetFloat("_InvertY", -1);
					} else {
						material.SetFloat("_InvertY", 1);
					}

				break;
			}

			material.SetPass(0);

			GLExtended.DrawMesh(meshObjects, position, id.mainShape.transform2D.scale, id.mainShape.transform2D.rotation);

			material.mainTexture = null;
		}
    }
}
