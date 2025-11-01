using __SCRIPTS;
using UnityEngine;

public class AdvanceToNextLevelTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public SceneDefinition nextScene;

    private bool hasCompleted;

    private void OnTriggerEnter2D(Collider2D other)
    {
	    if (hasCompleted) return;
	     var life = other.GetComponent<Life>();
	     if (life == null) return;
	     if (!life.IsHuman) return;
	     Debug.Log("player reached the end of the level");
	    hasCompleted = true;
	     Services.levelManager.AdvanceToNextLevel(nextScene);
    }
}
