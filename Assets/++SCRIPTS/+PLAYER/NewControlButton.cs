using System;

namespace __SCRIPTS
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