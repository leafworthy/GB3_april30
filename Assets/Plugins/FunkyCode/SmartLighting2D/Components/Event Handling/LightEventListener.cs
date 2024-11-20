using System.Collections.Generic;
using System.Linq;
using FunkyCode.SmartLighting2D.Scripts.Event_Handling;
using UnityEngine;

namespace FunkyCode
{
	[ExecuteInEditMode]
	public class LightEventListener : MonoBehaviour
	{
		public float visibility = 0;

		public LightCollision2D? CollisionInfo = null;

		private LightCollider2D lightCollider;
		public LightCollision2D colliderrr;

		[SerializeField]
		private Dictionary<Light2D, LightCollision2D> currentCollisions = new Dictionary<Light2D, LightCollision2D>();

		private List<LightCollision2D> collisionList = new List<LightCollision2D>();

		private List<Light2D> toRemove = new List<Light2D>();

		private void OnEnable()
		{
			lightCollider = GetComponent<LightCollider2D>();

			lightCollider?.AddEvent(CollisionEvent);
		}

		private void OnDisable()
		{
			currentCollisions.Clear();
			lightCollider?.RemoveEvent(CollisionEvent);
		}

		private void CollisionEvent(LightCollision2D collision)
		{
			if (currentCollisions.ContainsKey(collision.light))
			{
				currentCollisions[collision.light] = collision;
				colliderrr = collision;
				return;
			}

			currentCollisions.Add(collision.light, collision);
			collisionList.Add(collision);
			colliderrr = collision;
		}

		private void Update()
		{
			visibility = 0;
			if (currentCollisions.Count <= 0) return;
			foreach (var light2D in currentCollisions)
			{
				if (light2D.Key == null)
				{
					toRemove.Add(light2D.Key);
					continue;
				}
				if ((light2D.Value.points == null) || (!light2D.Key.isActiveAndEnabled))
				{
					toRemove.Add(light2D.Key);
					continue;
				}

				if (light2D.Value.points.Count <= 0)
				{
					toRemove.Add(light2D.Key);
					continue;
				}

				var distance = Vector2.Distance(light2D.Key.transform.position, transform.position);
				if (distance > light2D.Key.size)
				{
					toRemove.Add(light2D.Key);
					continue;
				}

				visibility += Mathf.Max(0, light2D.Key.size - distance);
			}

			foreach (var key in toRemove)
			{
				currentCollisions.Remove(key);
				var item = collisionList.FirstOrDefault((v) => v.light == key);
				collisionList.Remove(item);
			}
			toRemove.Clear();
		}
	}
}
