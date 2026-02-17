using __SCRIPTS.Cursor;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Video;


namespace __SCRIPTS
{
	public class CrimsonBuckTransition : MonoBehaviour
	{
		bool hasDoneTransition;
		[SerializeField] LayerMask playerLayer;
		public SceneDefinition crimsonCinematicScene;



		void OnTriggerEnter2D(Collider2D other)
		{
			Debug.Log("trigger enter");
			if ((1 << other.gameObject.layer & playerLayer) == 0) return;

			DoTransition();
		}

		void DoTransition()
		{
			if (hasDoneTransition) return;
			hasDoneTransition = true;
			Debug.Log("triggered");
			Services.sceneLoader.GoToScene(crimsonCinematicScene);
			Services.hudManager.HideHUD();
		}



	}
}
