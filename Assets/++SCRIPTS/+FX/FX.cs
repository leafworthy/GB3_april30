using UnityEngine;

[ExecuteInEditMode]
public class FX : Singleton<FX>
{
	public static FXAssets Assets => I._fx ? I._fx : Resources.Load<FXAssets>("Assets/FX");
	private FXAssets _fx;
}