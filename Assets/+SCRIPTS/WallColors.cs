using __SCRIPTS;
using UnityEngine;

[ExecuteInEditMode]
public class WallColors : MonoBehaviour
{
	public Color wallColor = Color.white;
	public Color trimColor = Color.white;
	public Color glassColor = Color.white;
	public GenericCharacterBuilder wallColorGenericCharacterBuilder;
	public GenericCharacterBuilder trimColorGenericCharacterBuilder;
	public GenericCharacterBuilder glassColorGenericCharacterBuilder;

	void OnEnable()
	{
		Refresh();
	}

	void Update()
	{
		Refresh();
	}

	void Refresh()
	{
		if (wallColorGenericCharacterBuilder != null) wallColorGenericCharacterBuilder.Tint = wallColor;

		if (trimColorGenericCharacterBuilder != null) trimColorGenericCharacterBuilder.Tint = trimColor;

		if (glassColorGenericCharacterBuilder != null) glassColorGenericCharacterBuilder.Tint = glassColor;
	}
}
