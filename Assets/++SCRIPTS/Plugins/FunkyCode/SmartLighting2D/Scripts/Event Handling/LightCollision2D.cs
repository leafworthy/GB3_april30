using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Light;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Components.Night;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Event_Handling
{
	[System.Serializable]
	public struct LightCollision2D
	{
		public enum State
		{
			OnCollision,
			OnCollisionEnter,
			OnCollisionExit
		}

		public Light2D light;
		public LightCollider2D collider;

		public List<Vector2> points;
		public State state;

		public LightCollision2D(bool useInfo, State state)
		{
			this.light = null;

			this.collider = null;

			this.points = null;

			this.state = state;
		}
	}
}
