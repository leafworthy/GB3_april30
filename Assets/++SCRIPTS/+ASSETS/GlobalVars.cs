using System;
using UnityEngine;

namespace __SCRIPTS
{
	[Serializable,CreateAssetMenu(menuName = "My Assets/GlobalVars")]
	public class GlobalVars : ScriptableObject
	{
		public int GasGoal = 20;
		public Vector3 Gravity = new(0, 4.5f, 0);
	}
}