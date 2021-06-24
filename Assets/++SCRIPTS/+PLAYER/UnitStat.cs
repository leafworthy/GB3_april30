using System;

[Serializable]
public class UnitStat
{
	public StatType type;
	public float value;
	private float originalValue;
	private bool isSet;

	public UnitStat(UnitStat stat)
	{
		this.type = stat.type;
		this.value = stat.GetValue();
		this.originalValue = stat.originalValue;
		this.isSet = stat.isSet;
	}

	public float GetValue()
	{
		Set();
		return value;
	}

	private void Set()
	{
		if (isSet) return;
		isSet = true;
		originalValue = value;
	}

	public void ChangeValue(float changeInValue)
	{
		Set();
		value += changeInValue;
	}

	public void SetValue(float newValue)
	{
		Set();
		value = newValue;
	}
	public void ResetValue()
	{
		Set();
		value = originalValue;
	}

	public float GetBaseValue()
	{
		Set();
		return originalValue;
	}
}
