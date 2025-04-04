﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Manager;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.Camera;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night
	{
	[ExecuteInEditMode]
	public class LightTexture2D : MonoBehaviour {
		public int lightLayer = 0;
		public Texture texture;
		public Color color = Color.white;
		public Vector2 size = Vector2.one;

		public enum ShaderMode {Additive, Multiply}

		public ShaderMode shaderMode = ShaderMode.Additive;

		public static List<LightTexture2D> List = new List<LightTexture2D>();

		public void OnEnable() {
			List.Add(this);

			LightingManager2D.Get();
		}

		public void OnDisable() {
			List.Remove(this);
		}

		public bool InCamera(Camera camera) {
			float cameraRadius = CameraTransform.GetRadius(camera);
			float distance = Vector2.Distance(transform.position, camera.transform.position);
			float radius = cameraRadius + Mathf.Sqrt((size.x * size.x) * (size.y * size.y));

			return(distance < radius);
		}
	}
}
