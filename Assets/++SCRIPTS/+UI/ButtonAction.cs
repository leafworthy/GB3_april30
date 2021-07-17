using UnityEngine.Events;

[System.Serializable]
public class ButtonAction
{
	public string Name;
	public UnityAction action;
	public ButtonAction(string name, UnityAction action)
	{
		this.Name = name;
		this.action = action;
	}
}
