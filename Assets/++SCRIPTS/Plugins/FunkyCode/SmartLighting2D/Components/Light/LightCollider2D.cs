﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightCollider2D;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Event_Handling;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Sprite_Mesh;
using __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2.Polygon2;
using UnityEngine;
using UnityEngine.Events;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light
{
	[ExecuteInEditMode]
	public class LightCollider2D : MonoBehaviour
	{
		public SpriteRenderer spriteForShape;
		public enum ShadowType {None, SpritePhysicsShape, CompositeCollider2D, Collider2D, Collider3D, MeshRenderer, SkinnedMeshRenderer};
		public enum MaskType {None, Sprite, BumpedSprite,  SpritePhysicsShape, CompositeCollider2D, Collider2D, Collider3D, MeshRenderer, BumpedMeshRenderer, SkinnedMeshRenderer};
		public enum MaskPivot {TransformCenter, ShapeCenter, LowestY};
		public enum ShadowDistance {Infinite, Finite}

		// shadow
		public ShadowType shadowType = ShadowType.SpritePhysicsShape;
		public int shadowLayer = 0;

		[Min(0)]
		public ShadowDistance shadowDistance = ShadowDistance.Infinite;

		[Min(0.1f)]
		public float shadowDistanceMin = 0.5f;

		[Min(0)]
		public float shadowDistanceMax = 1f;

		[Range(0, 1)]
		public float shadowTranslucency = 0;

		// mask
		public MaskType maskType = MaskType.None;
		public MaskLit maskLit = MaskLit.Lit;
		public MaskPivot maskPivot = MaskPivot.TransformCenter;
		public int maskLayer = 0;

		[Range(0, 1)]
		public float maskLitCustom = 1;

		public bool isStatic => gameObject.isStatic;

		public BumpMapMode bumpMapMode = new BumpMapMode();

		// event handling

		public event CollisionEvent2D collisionEvents;
		public LightEvent lightOnEnter;
		public LightEvent lightOnExit;

		// internal

		public LightColliderShape mainShape = new LightColliderShape();

		public SpriteMeshObject spriteMeshObject = new SpriteMeshObject();

		// list manager 
		private int listMaskLayer = -1;
		private int listShadowLayer = -1;

		public static List<LightCollider2D> List = new List<LightCollider2D>();
		public static List<LightCollider2D> ListEventReceivers = new List<LightCollider2D>();
		public static LightColliderLayer<LightCollider2D> layerManagerMask = new LightColliderLayer<LightCollider2D>();
		public static LightColliderLayer<LightCollider2D> layerManagerShadow = new LightColliderLayer<LightCollider2D>();

		public bool ShadowDisabled()
		{
			return(mainShape.shadowType == LightCollider2D.ShadowType.None);
		}

		public void AddEventOnEnter(UnityAction<Light2D> call)
		{
			if (lightOnEnter == null)
			{
				lightOnEnter = new LightEvent();
			}

			lightOnEnter.AddListener(call);
		}

		public void AddEventOnExit(UnityAction<Light2D> call)
		{
			if (lightOnExit == null)
			{
				lightOnExit = new LightEvent();
			}

			lightOnExit.AddListener(call);
		}

		public void AddEvent(CollisionEvent2D collisionEvent)
		{
			collisionEvents += collisionEvent;

			ListEventReceivers.Add(this);
		}

		public void RemoveEvent(CollisionEvent2D collisionEvent)
		{
			ListEventReceivers.Remove(this);

			collisionEvent -= collisionEvent;
		}

		public static void ForceUpdateAll()
		{
			foreach (LightCollider2D lightCollider2D in LightCollider2D.List)
			{
				lightCollider2D.Initialize();
			}
		}
		
		private void OnEnable()
		{
			List.Add(this);

			UpdateLayerList();

			LightingManager2D.Get();

			Initialize();

			UpdateNearbyLights();

			bumpMapMode.SetSpriteRenderer(mainShape.spriteShape.GetSpriteRenderer(spriteForShape));
		}

		private void OnDisable()
		{
			List.Remove(this);
			
			ClearLayerList();
			
			UpdateNearbyLights();
		}

		private void OnDestroy()
		{
			if (!Application.isPlaying) return;
			List.Remove(this);
			
			if (ListEventReceivers.Count > 0)
			{
				if (ListEventReceivers.Contains(this))
				{
					ListEventReceivers.Remove(this);
				}
			}
		}

		// Layer List
		void ClearLayerList()
		{
			layerManagerMask.Remove(listMaskLayer, this);
			layerManagerShadow.Remove(listShadowLayer, this);

			listMaskLayer = -1;
			listShadowLayer = -1;
		}

		void UpdateLayerList()
		{
			listMaskLayer = layerManagerMask.Update(listMaskLayer, maskLayer, this);
			listShadowLayer = layerManagerShadow.Update(listShadowLayer, shadowLayer, this);
		}

		static public List<LightCollider2D> GetMaskList(int layer) => layerManagerMask.layerList[layer];
		static public List<LightCollider2D> GetShadowList(int layer) => layerManagerShadow.layerList[layer];

		public void CollisionEvent(LightCollision2D collision)
		{
			if (collisionEvents != null)
			{
				collisionEvents(collision);
			}
		}

		public bool InLight(Light2D light) => mainShape.RectOverlap(light.transform2D.WorldRect);

		// light 2D method?
		// light 2D should know what layers id's it is supposed to draw? (include in array)
		
		public void UpdateNearbyLights()
		{
			for(int id = 0; id < Light2D.List.Count; id++)
			{
				Light2D light = Light2D.List[id];
			
				if (!light.IfDrawLightCollider(this))
				{
					continue;
				}

				if (InLight(light))
				{
					light.ForceUpdate();
				}
			}
		}

		public void Initialize()
		{
			mainShape.maskType = maskType;
			mainShape.maskPivot = maskPivot;

			mainShape.shadowType = shadowType;

			mainShape.SetTransform(this);
			mainShape.transform2D.Reset();
			mainShape.transform2D.Update(true);
			mainShape.transform2D.UpdateNeeded = true;

			mainShape.ResetLocal();			
		}

		public void UpdateLoop()
		{
			UpdateLayerList();

			if (isStatic)
			{
				return;
			}

			bool updateLights = false;

			mainShape.transform2D.Update(false);

			if (mainShape.transform2D.UpdateNeeded)
			{
				mainShape.transform2D.UpdateNeeded = false;
				
				mainShape.ResetWorld();

				updateLights = true;
			}

			if (updateLights)
			{
				UpdateNearbyLights();
			}
		}

		void OnDrawGizmosSelected()
		{
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Selected)
			{
				return;
			}

			DrawGizmos();
		}

		private void OnDrawGizmos()
		{
			if (Lighting2D.ProjectSettings.editorView.drawGizmos != EditorDrawGizmos.Always)
			{
				return;
			}

			DrawGizmos();
		}
		
		private void DrawGizmos()
		{
			if (!isActiveAndEnabled)
			{
				return;
			}

			switch(Lighting2D.ProjectSettings.editorView.drawGizmosShadowCasters)
			{
				case EditorShadowCasters.Enabled:

					Gizmos.color = new Color(1f, 0.5f, 0.25f);
		
					if (mainShape.shadowType != LightCollider2D.ShadowType.None)
					{
						List<Polygon2> polygons = mainShape.GetPolygonsWorld();
						
						GizmosHelper.DrawPolygons(polygons, transform.position);
					}

				break;
			}

			switch(Lighting2D.ProjectSettings.editorView.drawGizmosBounds)
			{
				case EditorGizmosBounds.Enabled:
				
					if (maskLit == MaskLit.Isometric)
					{
						Gizmos.color = Color.green;
						GizmosHelper.DrawIsoRect(transform.position, mainShape.GetIsoWorldRect());
					}
						else
					{
						Gizmos.color = new Color(0, 1f, 1f, 0.5f);
						GizmosHelper.DrawRect(transform.position, mainShape.GetWorldRect());
					}
					
				break;
			}

			if (Lighting2D.ProjectSettings.editorView.drawIcons == EditorIcons.Enabled)
			{
				Vector2? pivotPoint = mainShape.GetPivotPoint();

				if (pivotPoint != null)
				{
					Vector3 pos = transform.position;
					pos.x = pivotPoint.Value.x;
					pos.y = pivotPoint.Value.y;

					Gizmos.DrawIcon(pos, "circle_v2", true);
				}
			}
		}
	}
}
