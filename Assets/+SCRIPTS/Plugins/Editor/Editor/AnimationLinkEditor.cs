using System.Collections.Generic;
using __SCRIPTS.Plugins.AstarPathfindingProject.Core.Misc;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins.Editor.Editor {
	[CustomEditor(typeof(AnimationLink))]
	public class AnimationLinkEditor : UnityEditor.Editor {
		public override void OnInspectorGUI () {
			DrawDefaultInspector();

			var script = target as AnimationLink;

			EditorGUI.BeginDisabledGroup(script.EndTransform == null);
			if (GUILayout.Button("Autoposition Endpoint")) {
				List<Vector3> buffer = ListPool<Vector3>.Claim();
				Vector3 endpos;
				script.CalculateOffsets(buffer, out endpos);
				script.EndTransform.position = endpos;
				ListPool<Vector3>.Release(buffer);
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
