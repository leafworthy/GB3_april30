﻿
namespace __SCRIPTS.Plugins.FunkyCode.SmartUtilities2D.Scripts.Utilities._2D
{
	public class Mathd
	{
		public static double Distance(double x1, double y1, double x2, double y2)
		{
			return System.Math.Sqrt(System.Math.Pow((x2 - x1), 2) + System.Math.Pow((y2 - y1), 2));
		}
	}
}