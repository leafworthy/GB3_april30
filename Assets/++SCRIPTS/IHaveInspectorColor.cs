using UnityEngine;

public abstract class IHaveInspectorColor: MonoBehaviour
{
	public abstract Color GetBackgroundColor();

	public abstract string GetIconPath();
}