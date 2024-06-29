using System;
using UnityEngine;

[Serializable]
public class UnitStat
{
	public float value;
	private float originalValue;
	private bool originalValueHasBeenSet;
	public float maxValue;
	public float minValue;
	public bool isLimited = false;

	public UnitStat(UnitStat stat)
	{
		value = stat.GetValue();
		maxValue = stat.maxValue;
		minValue = stat.minValue;
		isLimited = stat.isLimited;
		SetOriginalValue();
	}

	public float GetValue()
	{
		return value;
	}

	private void SetOriginalValue()
	{
		if (originalValueHasBeenSet) return;
		originalValueHasBeenSet = true;
		originalValue = value;
	}

	public void ChangeValue(float changeInValue)
	{
		value += changeInValue;
		LimitValue();
	}

	private void LimitValue()
	{

		if (!isLimited) return;
		value = Mathf.Min(value, maxValue);
		value = Mathf.Max(value, minValue);
	}

	public void SetValue(float newValue)
	{
		value = newValue;
		LimitValue();
	}
	public void ResetValue()
	{
		value = originalValue;
		LimitValue();
	}

	public float GetBaseValue()
	{
		return originalValue;
	}
}
