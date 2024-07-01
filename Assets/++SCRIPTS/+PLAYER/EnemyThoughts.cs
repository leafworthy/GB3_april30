using TMPro;
using UnityEngine;

namespace __SCRIPTS._PLAYER
{
	public class EnemyThoughts : MonoBehaviour
	{
		private TextMeshProUGUI text => GetComponentInChildren<TextMeshProUGUI>();
		public void Think(string thought)
		{
			if (text == null) return;
			text.text = thought;
		}
	}
}