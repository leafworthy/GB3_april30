public class Counter
{
	private int counter;
	private int howOften;
	public Counter(int _howOften)
	{
		howOften = _howOften;
	}

	public bool ShouldReturn()
	{
		counter++;
		if (counter >= howOften)
		{
			counter = 0;
			return false;
		}

		return true;
	}
}