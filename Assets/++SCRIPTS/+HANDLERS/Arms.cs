using System;

[Serializable]
public class Arms : ActivityHandler
{
}

[Serializable]
public class Legs : ActivityHandler
{
	}

[Serializable]
public class ActivityHandler
{
	public string currentActivity;
	public bool isActive;


	public bool Do(string Verb)
	{
		if (isActive)
		{
			return false;
		}
		isActive = true;
		currentActivity = Verb;
		return true;
	}


	public bool Stop(string Verb)
	{
		if (!isActive)
		{
			return false;
		}
		isActive = false;
		currentActivity = null;
		return true;
	}
}
