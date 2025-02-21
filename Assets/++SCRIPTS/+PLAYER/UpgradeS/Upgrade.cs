using System;
using UnityEngine;

[Serializable]
public class Upgrade:MonoBehaviour
{
	protected int level = 1;
	protected int cost;

	public virtual void CauseEffect(Player player)
	{
	}
	
	public virtual void UpgradeLevel()
	{
		level++;
	}

	public virtual int GetCost()
	{
		return cost* level;
	}

	public int GetLevel()
	{
		return level;
	}

	public virtual string GetName()
	{
		return "";
	}
	
	public virtual void ResetLevel()
	{
		level = 1;
	}
}