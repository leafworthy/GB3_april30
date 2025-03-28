﻿using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Settings.Presets;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Event_Handling
{
    public class Object
	{
        public List<LightCollider2D> listenersCache = new List<LightCollider2D>();

		public List<LightCollision2D> listenersInLight = new List<LightCollision2D>();
		public List<LightCollider2D> listenersInLightColliders = new List<LightCollider2D>();

		public void Update(Light2D light, EventPreset eventPreset)
		{
			listenersInLight.Clear();

			// Get Event Receivers
			LightCollider.GetCollisions(listenersInLight, light);

			// Remove Event Receiver Vertices with Shadows
			LightCollider.RemoveHiddenPoints(listenersInLight, light, eventPreset);
			LightTilemap.RemoveHiddenPoints(listenersInLight, light, eventPreset);

			if (listenersInLight.Count < 1)
			{
				for(int i = 0; i < listenersCache.Count; i++)
				{
					LightCollider2D collider = listenersCache[i];

					LightCollision2D collision = new LightCollision2D();
					collision.light = light;
					collision.collider = collider;
					collision.points = null;
					collision.state = LightCollision2D.State.OnCollisionExit;

					collider.CollisionEvent(collision);
				}

				listenersCache.Clear();

				return;
			}

			listenersInLightColliders.Clear();

			foreach(LightCollision2D collision in listenersInLight)
			{
				listenersInLightColliders.Add(collision.collider);
			}

			for(int i = 0; i < listenersCache.Count; i++)
			{
				LightCollider2D collider = listenersCache[i];

				if (!listenersInLightColliders.Contains(collider))
				{
					LightCollision2D collision = new LightCollision2D();
					collision.light = light;
					collision.collider = collider;
					collision.points = null;
					collision.state = LightCollision2D.State.OnCollisionExit;

					collider.CollisionEvent(collision);

					listenersCache.Remove(collider);
				}
			}

			for(int i = 0; i < listenersInLight.Count; i++)
			{
				LightCollision2D collision = listenersInLight[i];

				if (listenersCache.Contains(collision.collider))
				{
					collision.state = LightCollision2D.State.OnCollision;
				}
					else
				{
					collision.state = LightCollision2D.State.OnCollisionEnter;
					listenersCache.Add(collision.collider);
				}

				collision.collider.CollisionEvent(collision);
			}
		}
	}
}
