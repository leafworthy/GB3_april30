using System;

namespace GangstaBean.Player
{
	public interface NewControlButton 
	{
		public Player owner { get; set; }
		public event Action<NewControlButton> OnPress;
		public event Action<NewControlButton> OnHold;
		public event Action<NewControlButton> OnRelease;
		bool IsPressed { get; }
	}
}