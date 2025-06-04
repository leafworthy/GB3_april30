using TMPro;
using UnityEngine;

namespace GangstaBean.EnemyAI._ENEMYAI
{
	public class EnemyThoughts : MonoBehaviour
	{
		private string allThoughts;
		private TextMeshProUGUI text => GetComponentInChildren<TextMeshProUGUI>();

		private void Start()
		{
			allThoughts = "Thoughts for " + name + ": \n";
		}

		public void Think(string thought)
		{
			if (text == null) return;
			text.text = thought;
			allThoughts += thought + "\n";
		}

		private void OnDisable()
		{
		
		}
	}
}