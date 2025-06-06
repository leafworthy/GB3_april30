using __SCRIPTS.Plugins.AstarPathfindingProject.Core;
using __SCRIPTS.Plugins.AstarPathfindingProject.Utilities;
using __SCRIPTS.Plugins.Editor.ASTARPATH.Editor;
using UnityEditor;
using UnityEngine;

namespace __SCRIPTS.Plugins.Editor.Editor {
	[CustomEditor(typeof(ProceduralGridMoveAbility))]
	[CanEditMultipleObjects]
	public class ProceduralGridMoveAbilityEditor : EditorBase {
		GUIContent[] graphLabels = new GUIContent[32];

		protected override void Inspector () {
			// Make sure the AstarPath object is initialized and the graphs are loaded, this is required to be able to show graph names in the mask popup
			AstarPath.FindAstarPath();

			for (int i = 0; i < graphLabels.Length; i++) {
				if (AstarPath.active == null || AstarPath.active.data.graphs == null || i >= AstarPath.active.data.graphs.Length || AstarPath.active.data.graphs[i] == null) {
					graphLabels[i] = new GUIContent("Graph " + i + (i == 31 ? "+" : ""));
				} else {
					graphLabels[i] = new GUIContent(AstarPath.active.data.graphs[i].name + " (graph " + i + ")");
				}
			}

			Popup("graphIndex", graphLabels, "Graph");
			PropertyField("target");
			PropertyField("updateDistance");
		}
	}
}
