using System;

internal interface IShootHandler
{
	event Action<Attack> OnShootStart;
	bool IsGlocking { get; }
}
