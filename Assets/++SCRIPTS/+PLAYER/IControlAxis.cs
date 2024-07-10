using System;
using UnityEngine;

public interface IControlAxis 
{
	public Player owner { get; set; }
	public event Action<NewInputAxis,Vector2> OnChange;
	public event Action<NewInputAxis> OnActive;
	public event Action<NewInputAxis> OnInactive;
	public event Action<NewInputAxis> OnRight;
	public event Action<NewInputAxis> OnLeft;
	public event Action<NewInputAxis> OnUp;
	public event Action<NewInputAxis> OnDown;
	Vector3 GetCurrentAngle();
}