using System;

namespace _PLUGINS.NaughtyAttributes.Scripts.Core.DrawerAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AllowNestingAttribute : DrawerAttribute
	{
	}
}
