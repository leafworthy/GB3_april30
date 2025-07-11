using __SCRIPTS.Plugins.AstarPathfindingProject.Core.Serialization;
using __SCRIPTS.Plugins.AstarPathfindingProject.Generators;

namespace __SCRIPTS.Plugins.AstarPathfindingProject.Core.Misc {
	[JsonOptIn]
	/// <summary>Defined here only so non-editor classes can use the <see cref="target"/> field</summary>
	public class GraphEditorBase {
		/// <summary>NavGraph this editor is exposing</summary>
		public NavGraph target;
	}
}
