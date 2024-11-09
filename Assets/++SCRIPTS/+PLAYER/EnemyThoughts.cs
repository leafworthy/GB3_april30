using TMPro;
using UnityEngine;

public class EnemyThoughts : MonoBehaviour
{
	private TextMeshProUGUI text => GetComponentInChildren<TextMeshProUGUI>();
	public void Think(string thought)
	{
		Debug.Log("thought: " + thought);
		if (text == null) return;
		text.text = thought;
	}
}