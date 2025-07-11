using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightCollider2D;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Universal.Objects;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Rendering.Light.Mask
{
    public class SkinnedMesh
	{
        public static void Mask(Light2D light, LightCollider2D id, UnityEngine.Material material, LayerSetting layerSetting)
		{
			if (!id.InLight(light))
			{
				return;
			}

			LightColliderShape shape = id.mainShape;

			SkinnedMeshRenderer skinnedMeshRenderer = shape.skinnedMeshShape.GetSkinnedMeshRenderer();

			if (skinnedMeshRenderer == null)
			{
				return;
			}

			List<MeshObject> meshObject = shape.GetMeshes();

			if (meshObject == null)
			{
				return;
			}

			if (skinnedMeshRenderer.sharedMaterial != null)
			{
				material.mainTexture = skinnedMeshRenderer.sharedMaterial.mainTexture;
			}
				else
			{
				material.mainTexture = null;
			}

			Vector2 position = shape.transform2D.position - light.transform2D.position;

			Vector2 pivotPosition = shape.GetPivotPoint() - light.transform2D.position;
			GLExtended.color = LayerSettingColor.Get(pivotPosition, layerSetting, id.maskLit, 1, id.maskLitCustom);

			material.SetPass(0);

			GLExtended.DrawMesh(meshObject, position, id.mainShape.transform2D.scale, shape.transform2D.rotation);

			material.mainTexture = null;
		}
    }
}
