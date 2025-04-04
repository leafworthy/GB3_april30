﻿using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Day;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Effects;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Occlusion;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace __SCRIPTS.Plugins.Editor.FUNKYCODE1.Editor.Misc
{
	public class EditorGameObjects : MonoBehaviour
	{
		static public UnityEngine.Camera GetCamera()
		{
			UnityEngine.Camera camera = null;

			if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
			{
				camera = SceneView.lastActiveSceneView.camera;
			}
				else if (UnityEngine.Camera.main != null)
			{
				camera = UnityEngine.Camera.main;
			}
			
			return(camera);
		}

		static public Vector3 GetCameraPoint()
		{
			Vector3 pos = Vector3.zero;

			UnityEngine.Camera camera = GetCamera();

			if (camera != null)
			{
				Ray worldRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
				pos = worldRay.origin;
				pos.z = 0;
			}
				else
			{
				UnityEngine.Debug.LogError("Scene Camera Not Found");
			}

			return(pos);
		}

		[MenuItem("GameObject/2D Light/Light/Light", false, 4)]
		static void CreateLightSource()
		{	
			GameObject newGameObject = new GameObject("Light 2D");

			newGameObject.AddComponent<Light2D>();

			newGameObject.transform.position = GetCameraPoint();
		}

		[MenuItem("GameObject/2D Light/Collider/Light Collider", false, 4)]
		static void CreateLightCollider()
		{
			GameObject newGameObject = new GameObject("Light Collider 2D");

			newGameObject.AddComponent<PolygonCollider2D>();
			LightCollider2D collider = newGameObject.AddComponent<LightCollider2D>();
			collider.maskType = LightCollider2D.MaskType.Collider2D;
			collider.shadowType = LightCollider2D.ShadowType.Collider2D;
			collider.Initialize();

			newGameObject.transform.position = GetCameraPoint();
		}

		[MenuItem("GameObject/2D Light/Collider/Light Tilemap Collider", false, 4)]
		static void CreateLightTilemapCollider()
		{
			GameObject newGrid = new GameObject("2D Light Grid");
			newGrid.AddComponent<Grid>();

			GameObject newGameObject = new GameObject("2D Light Tilemap");
			newGameObject.transform.parent = newGrid.transform;

			newGameObject.AddComponent<Tilemap>();
			newGameObject.AddComponent<LightTilemapCollider2D>();
		}

		[MenuItem("GameObject/2D Light/Light/Light Sprite", false, 4)]
		static void CreateLightSpriteRenderer()
		{
			GameObject newGameObject = new GameObject("Light Sprite 2D");
			
			LightSprite2D spriteRenderer2D = newGameObject.AddComponent<LightSprite2D>();
			spriteRenderer2D.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/gfx_light");

			newGameObject.transform.position = GetCameraPoint();
		}

		[MenuItem("GameObject/2D Light/Light/Light Texture", false, 4)]
		static void CreateLightTextureRenderer() {
			GameObject newGameObject = new GameObject("Light Texture 2D ");
			
			LightTexture2D textureRenderer = newGameObject.AddComponent<LightTexture2D>();
			textureRenderer.texture = UnityEngine.Resources.Load<Texture>("Sprites/gfx_light");

			newGameObject.transform.position = GetCameraPoint();
		}

		[MenuItem("GameObject/2D Light/Collider/Day Light Collider", false, 4)]
		static void CreateDayLightCollider()
		{
			GameObject newGameObject = new GameObject("DayLight Collider 2D");

			newGameObject.AddComponent<PolygonCollider2D>();

			DayLightCollider2D c = newGameObject.AddComponent<DayLightCollider2D>();
			c.mainShape.shadowType = DayLightCollider2D.ShadowType.Collider2D;
			c.mainShape.maskType = DayLightCollider2D.MaskType.None;

			newGameObject.transform.position = GetCameraPoint();
		}

		[MenuItem("GameObject/2D Light/Collider/Day Light Tilemap Collider", false, 4)]
		static void CreateDayLightTilemapCollider()
		{
			GameObject newGrid = new GameObject("Light Grid 2D");
			newGrid.AddComponent<Grid>();

			GameObject newGameObject = new GameObject("DayLight Tilemap 2D");
			newGameObject.transform.parent = newGrid.transform;

			newGameObject.AddComponent<Tilemap>();
			newGameObject.AddComponent<DayLightTilemapCollider2D>();
		}
		
		[MenuItem("GameObject/2D Light/Room/Light Room", false, 4)]
		static void CreateLightRoom()
		{
			GameObject newGameObject = new GameObject("Light Room 2D");

			newGameObject.AddComponent<PolygonCollider2D>();
			newGameObject.AddComponent<LightRoom2D>();

			newGameObject.transform.position = GetCameraPoint();
		}

		[MenuItem("GameObject/2D Light/Room/Light Tilemap Room", false, 4)]
		static void CreateLightTilemapRoom()
		{
			GameObject newGrid = new GameObject("2D Light Grid");
			newGrid.AddComponent<Grid>();

			GameObject newGameObject = new GameObject("Light Tilemap Room 2D");
			newGameObject.transform.parent = newGrid.transform;

			newGameObject.AddComponent<Tilemap>();
			newGameObject.AddComponent<LightTilemapRoom2D>();
		}

		[MenuItem("GameObject/2D Light/Occlusion/Light Occlusion", false, 4)]
		static void CreateLightOcclusion()
		{
			GameObject newGameObject = new GameObject("2D Light Occlusion");

			newGameObject.AddComponent<PolygonCollider2D>();
			newGameObject.AddComponent<LightOcclusion2D>();

			newGameObject.transform.position = GetCameraPoint();
		}

		[MenuItem("GameObject/2D Light/Occlusion/Light Tilemap Occlusion", false, 4)]
		static void CreateLightTilemapOcclusion()
		{
			GameObject newGrid = new GameObject("Light Grid 2D");
			newGrid.AddComponent<Grid>();

			GameObject newGameObject = new GameObject("Light Tilemap Occlusion 2D");
			newGameObject.transform.parent = newGrid.transform;

			newGameObject.AddComponent<Tilemap>();
			newGameObject.AddComponent<LightTilemapOcclusion2D>();
		}

		[MenuItem("GameObject/2D Light/Light Manager", false, 4)]
		static void CreateLightManager()
		{
			LightingManager2D.Get();
		}

		[MenuItem("GameObject/2D Light/Light Cycle", false, 4)]
		static void CreateLightCycle()
		{	
			GameObject newGameObject = new GameObject("Light Cycle 2D");

			newGameObject.AddComponent<LightCycle>();

			newGameObject.transform.position = GetCameraPoint();
		}
	}
}
