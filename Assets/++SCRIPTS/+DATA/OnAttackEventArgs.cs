using System;
using UnityEngine;

public class OnAttackEventArgs : EventArgs
{
	public OnAttackEventArgs(Vector3 attackStartPos, Vector3 attackEndPos, DefenceHandler hitObj = null)
	{
		AttackStartPosition = attackStartPos;
		AttackEndPosition = attackEndPos;
		hitObject = hitObj;
	}
	public Vector3 AttackStartPosition;
	public Vector3 AttackEndPosition;
	public DefenceHandler hitObject;
}
