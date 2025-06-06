﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Material;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager
{
	[ExecuteInEditMode]
	public class LightingMeshRenderer : LightingMonoBehaviour
	{
		public static List<LightingMeshRenderer> list = new List<LightingMeshRenderer>();

		public bool free = true;
		public UnityEngine.Object owner = null;

		public MeshRenderer meshRenderer = null;
		public MeshFilter meshFilter = null;

		private Material[] materials = new Material[1];

		public MeshModeShader meshModeShader = MeshModeShader.Additive;
		public Material[] meshModeMaterial = null;

		public Material[] GetMaterials()
		{
			if (materials == null)
			{
				materials = new Material[1];
			}

			if (materials.Length < 1)
			{
				materials = new Material[1];
			}

			switch(meshModeShader) {
				case MeshModeShader.Additive:

					if (materials[0] == null)
					{
						materials[0] = LightingMaterial.Load("Light2D/Internal/MeshModeAdditive").Get();
					}

				break;

				case MeshModeShader.Alpha:

					if (materials[0] == null)
					{
						materials[0] = LightingMaterial.Load("Light2D/Internal/MeshModeAlpha").Get();
					}

				break;

				case MeshModeShader.Custom:

					materials = meshModeMaterial;

				break;
			}

			return(materials);
		}

		static public int GetCount()
		{
			return(list.Count);
		}

		public void OnEnable()
		{
			list.Add(this);
		}

		public void OnDisable()
		{
			list.Remove(this);
		}

		static public List<LightingMeshRenderer> List => list;

		public void Initialize()
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();

			// Mesh System?
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			meshRenderer.receiveShadows = false;
			meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			meshRenderer.allowOcclusionWhenDynamic = false;
		}

		public void Free()
		{
			owner = null;
			free = true;

			meshRenderer.enabled = false;

			if (meshRenderer.sharedMaterial != null)
			{
				meshRenderer.sharedMaterial.mainTexture = null;
			}
		}

		public void LateUpdate() {
			if (owner == null) {
				Free();
				return;
			}

			if (IsRendered())
			{
				meshRenderer.enabled = true;
			}
				else
			{
				Free();
				meshRenderer.enabled = false;
			}
		}

		public bool IsRendered()
		{
			string type = owner.GetType().ToString();

			switch(type)
			{
				case "Light2D":

					Light2D light = (Light2D)owner;
					if (light)
					{
						return(light.meshMode.enable && light.isActiveAndEnabled && light.InCameras());
					}

				break;

				case "LightSprite2D":

					LightSprite2D sprite = (LightSprite2D)owner;
					if (sprite)
					{
						return(sprite.meshMode.enable && sprite.isActiveAndEnabled);
					}

				break;
			}

			return(false);
		}

		public void ClearMaterial()
		{
			materials = new Material[1];
		}

		public void UpdateLight(Light2D id, MeshMode meshMode)
		{
			// Camera
			if (meshModeMaterial != meshMode.materials)
			{
				meshModeMaterial = meshMode.materials;

				ClearMaterial();
			}

			if (meshModeShader != meshMode.shader)
			{
				meshModeShader = meshMode.shader;

				ClearMaterial();
			}

			Material[] materials = GetMaterials();

			if (materials == null)
			{
				return;
			}

			if (id.IsPixelPerfect())
			{
				Camera camera = Camera.main;

				Vector2 cameraSize = LightingRender2D.GetSize(camera);
				Vector2 cameraPosition = LightingPosition.GetPosition2D(-camera.transform.position);

				transform.position = new Vector3(cameraPosition.x, cameraPosition.y, id.transform.position.z);

				transform.localScale = new Vector3(cameraSize.x, cameraSize.y, 1);
			}
				else
			{
				transform.position = id.transform.position;

				transform.localScale = new Vector3(id.size, id.size, 1);
			}

			transform.rotation = Quaternion.Euler(0, 0, 0);
			// transform.rotation = id.transform.rotation; // only if rotation enabled

			if (id.Buffer != null && meshRenderer != null)
			{
				Color lightColor = id.color;
				lightColor.a = id.meshMode.alpha;

				for(int i = 0; i < materials.Length; i++)
				{
					if (materials[i] == null) {
						continue;
					}

					materials[i].SetColor ("_Color", lightColor);
					materials[i].color = lightColor;
					materials[i].SetTexture("_Sprite", id.GetSprite().texture);

					materials[i].mainTexture = id.Buffer.renderTexture.renderTexture;
				}

				id.meshMode.sortingLayer.ApplyToMeshRenderer(meshRenderer);

				meshRenderer.sharedMaterials = GetMaterials();

				meshRenderer.enabled = true;

				meshFilter.mesh = GetMeshLight();
			}
		}

		public void UpdateLightSprite(LightSprite2D id, MeshMode meshMode)
		{
			if (id.GetSprite() == null)
			{
				Free();
				return;
			}

			if (meshModeMaterial != meshMode.materials)
			{
				meshModeMaterial = meshMode.materials;

				ClearMaterial();
			}

			if (meshModeShader != meshMode.shader)
			{
				meshModeShader = meshMode.shader;

				ClearMaterial();
			}

			Material[] material = GetMaterials();

			if (material == null)
			{
				return;
			}

			float rotation = id.lightSpriteTransform.rotation;
			if (id.lightSpriteTransform.applyRotation)
			{
				rotation += id.transform.rotation.eulerAngles.z;
			}

			////////////////////// Scale
			Vector2 scale = Vector2.zero;

			Sprite sprite = id.GetSprite();

			Rect spriteRect = sprite.textureRect;

			scale.x = (float)sprite.texture.width / spriteRect.width;
			scale.y = (float)sprite.texture.height / spriteRect.height;

			Vector2 size = id.lightSpriteTransform.scale;

			size.x *= 2;
			size.y *= 2;

			size.x /= scale.x;
			size.y /= scale.y;

			size.x *= (float)sprite.texture.width / (sprite.pixelsPerUnit * 2);
			size.y *= (float)sprite.texture.height / (sprite.pixelsPerUnit * 2);

			if (id.spriteRenderer.flipX) {
				size.x = -size.x;
			}

			if (id.spriteRenderer.flipY) {
				size.y = -size.y;
			}

			////////////////////// PIVOT
			Rect rect = spriteRect;
			Vector2 pivot = sprite.pivot;

			pivot.x /= spriteRect.width;
			pivot.y /= spriteRect.height;
			pivot.x -= 0.5f;
			pivot.y -= 0.5f;


			pivot.x *= size.x;
			pivot.y *= size.y;


			float pivotDist = Mathf.Sqrt(pivot.x * pivot.x + pivot.y * pivot.y);
			float pivotAngle = Mathf.Atan2(pivot.y, pivot.x);

			float rot = rotation * Mathf.Deg2Rad + Mathf.PI;

			Vector2 position = Vector2.zero;

			// Pivot Pushes Position

			position.x += Mathf.Cos(pivotAngle + rot) * pivotDist * id.transform.lossyScale.x;
			position.y += Mathf.Sin(pivotAngle + rot) * pivotDist * id.transform.lossyScale.y;
			position.x += id.transform.position.x;
			position.y += id.transform.position.y;
			position.x += id.lightSpriteTransform.position.x;
			position.y += id.lightSpriteTransform.position.y;

			Vector3 pos = position;
			pos.z = id.transform.position.z - 0.1f;
			transform.position = pos;

			Vector3 scale2 = id.transform.lossyScale;

			scale2.x *= size.x;
			scale2.y *= size.y;


			scale2.x /= 2;
			scale2.y /= 2;

			scale2.z = 1;

			transform.localScale = scale2;
			transform.rotation = Quaternion.Euler(0, 0, rotation);

			Rect uvRect = new Rect();
			uvRect.x = rect.x / sprite.texture.width;
			uvRect.y = rect.y / sprite.texture.height;
			uvRect.width = rect.width / sprite.texture.width + uvRect.x;
			uvRect.height = rect.height / sprite.texture.height + uvRect.y;

			if (meshRenderer != null) {
				Color lightColor = id.color;
				lightColor.a = id.meshMode.alpha;

				for(int i = 0; i < materials.Length; i++) {
					if (materials[i] == null) {
						continue;
					}

					materials[i].SetColor ("_Color", lightColor);
					materials[i].color = lightColor;
					materials[i].mainTexture = id.GetSprite().texture;
				}

				id.meshMode.sortingLayer.ApplyToMeshRenderer(meshRenderer);

				meshRenderer.sharedMaterials = materials;

				meshRenderer.enabled = true;

				Mesh mesh = GetMeshSprite();

				Vector2[] uvs = mesh.uv;
				uvs[0].x = uvRect.x;
				uvs[0].y = uvRect.y;

				uvs[1].x = uvRect.width;
				uvs[1].y = uvRect.y;

				uvs[2].x = uvRect.width;
				uvs[2].y = uvRect.height;

				uvs[3].x = uvRect.x;
				uvs[3].y = uvRect.height;

				mesh.uv = uvs;

				meshFilter.mesh = mesh;
			}
		}


		// Light Sprite Renderer
		public Mesh getSpriteMesh = null;
		public Mesh GetMeshSprite() {
			if (getSpriteMesh == null) {
				Mesh mesh = new Mesh();

				mesh.vertices = new Vector3[]{new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1)};
				mesh.triangles = new int[]{2, 1, 0, 0, 3, 2};
				mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

				getSpriteMesh = mesh;
			}
			return(getSpriteMesh);
		}

		// Light Source
		public Mesh getMeshLight = null;
		public Mesh GetMeshLight() {
			if (getMeshLight == null) {
				Mesh mesh = new Mesh();

				mesh.vertices = new Vector3[]{new Vector3(-1, -1), new Vector3(1, -1), new Vector3(1, 1), new Vector3(-1, 1)};
				mesh.triangles = new int[]{2, 1, 0, 0, 3, 2};
				mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

				getMeshLight = mesh;
			}
			return(getMeshLight);
		}

	}
}
