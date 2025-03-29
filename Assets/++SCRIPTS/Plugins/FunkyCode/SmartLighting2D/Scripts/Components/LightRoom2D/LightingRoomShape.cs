using System.Collections.Generic;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.LightShapes.Extensions;
using __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Misc;
using UnityEngine;

namespace __SCRIPTS.Plugins.FunkyCode.SmartLighting2D.Scripts.Components.LightRoom2D
{
	[System.Serializable]
	public class LightingRoomShape {
		public SmartLighting2D.Components.Night.LightRoom2D.RoomType type = SmartLighting2D.Components.Night.LightRoom2D.RoomType.Collider;

		public Collider2DShape colliderShape = new Collider2DShape();
		public SpriteShape spriteShape = new SpriteShape();

		public void SetTransform(Transform t) {
			colliderShape.SetTransform(t);
			spriteShape.SetTransform(t);
		}

		public void ResetLocal() {
			colliderShape.ResetLocal();

			spriteShape.ResetLocal();
		}

		public void ResetWorld() {
			colliderShape.ResetWorld();

			colliderShape.ResetWorld();
		}

		public List<MeshObject> GetMeshes() {
			switch(type) {
				case SmartLighting2D.Components.Night.LightRoom2D.RoomType.Collider:
					return(colliderShape.GetMeshes());

			}
		
			return(null);
		}

	}
}
