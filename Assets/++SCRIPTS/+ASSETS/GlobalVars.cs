using System;
using UnityEngine;

[Serializable,CreateAssetMenu(menuName = "My Assets/GlobalVars")]
public class GlobalVars : ScriptableObject
{
	public int GasGoal = 10;
	public Vector3 Gravity = new(0, 4.5f, 0);
}