using _PLUGINS.AstarPathfindingProject.Core.Serialization;
using _PLUGINS.AstarPathfindingProject.Generators;

namespace _PLUGINS.AstarPathfindingProject.Core.Misc {
	[JsonOptIn]
	/// <summary>Defined here only so non-editor classes can use the <see cref="target"/> field</summary>
	public class GraphEditorBase {
		/// <summary>NavGraph this editor is exposing</summary>
		public NavGraph target;
	}
}
